using UnityEngine;

public class AsteroidBelt : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private int maxAsteroids = 40;
    [SerializeField] private float spawnDistance = 100f;
    [SerializeField] private float despawnDistance = 12f;

    [Header("Appearance")]
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 5f;
    [SerializeField] private Color rockColor = new Color(0.4f, 0.38f, 0.35f, 1f);

    [Header("Movement")]
    [SerializeField] private float approachSpeed = 1.5f;
    [SerializeField] private float approachSpeedVariation = 1f;
    [SerializeField] private float wobbleStrength = 0.3f;
    [SerializeField] private float wobbleSpeed = 0.5f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Spawning")]
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private int spawnBatch = 2;

    [Header("Damage")]
    [SerializeField] private float damagePerUnit = 8f;

    private Transform cameraTransform;
    private float nextSpawnTime;

    private void Start()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
            cameraTransform = mainCam.transform;

        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnBatch();
            nextSpawnTime = Time.time + spawnInterval;
        }

        CleanupFarAsteroids();
    }

    private void SpawnBatch()
    {
        for (int i = 0; i < spawnBatch; i++)
        {
            if (transform.childCount >= maxAsteroids)
                break;

            Vector3 spawnPos = GetFarPosition();
            float size = Random.Range(minSize, maxSize);
            GameObject asteroid = CreateAsteroid(spawnPos, size);
            asteroid.transform.SetParent(transform);
        }
    }

    private Vector3 GetFarPosition()
    {
        Vector3 camPos = cameraTransform != null ? cameraTransform.position : Vector3.zero;
        Vector3 camForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;

        float angleH = Random.Range(-70f, 70f) * Mathf.Deg2Rad;
        float angleV = Random.Range(-40f, 40f) * Mathf.Deg2Rad;

        Vector3 dir = camForward;
        dir = Quaternion.AngleAxis(angleH * Mathf.Rad2Deg, Vector3.up) * dir;
        dir = Quaternion.AngleAxis(angleV * Mathf.Rad2Deg, Vector3.right) * dir;

        return camPos + dir * spawnDistance + Random.insideUnitSphere * 8f;
    }

    private void CleanupFarAsteroids()
    {
        Vector3 camPos = cameraTransform != null ? cameraTransform.position : Vector3.zero;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child == null)
                continue;

            float dist = Vector3.Distance(child.position, camPos);
            if (dist < despawnDistance || dist > spawnDistance * 2.5f)
            {
                MeshRenderer mr = child.GetComponent<MeshRenderer>();
                if (mr != null && mr.material != null)
                    Destroy(mr.material);

                Destroy(child.gameObject);
            }
        }
    }

    private GameObject CreateAsteroid(Vector3 position, float size)
    {
        GameObject asteroid = new GameObject("Asteroid");
        asteroid.transform.position = position;
        asteroid.transform.rotation = Random.rotation;

        CreateAsteroidMesh(asteroid, size);

        MeshRenderer mr = asteroid.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        float variation = Random.Range(0.7f, 1.3f);
        Color c = rockColor * variation;
        c.a = 1f;
        mr.material.color = c;
        mr.material.SetFloat("_Smoothness", Random.Range(0.02f, 0.15f));
        mr.material.SetFloat("_Metallic", Random.Range(0f, 0.08f));

        SphereCollider sphere = asteroid.AddComponent<SphereCollider>();
        sphere.isTrigger = true;
        sphere.radius = 0.7f;

        asteroid.isStatic = false;

        Vector3 camPos = cameraTransform != null ? cameraTransform.position : Vector3.zero;
        Vector3 toPlayer = (camPos - position).normalized;
        toPlayer += Random.insideUnitSphere * 0.2f;
        toPlayer.Normalize();

        float speed = approachSpeed + Random.Range(-approachSpeedVariation, approachSpeedVariation);
        float damage = size * damagePerUnit;

        AsteroidMover mover = asteroid.AddComponent<AsteroidMover>();
        mover.Initialize(toPlayer * speed, rotationSpeed, wobbleStrength, wobbleSpeed, damage);

        return asteroid;
    }

    private void CreateAsteroidMesh(GameObject asteroid, float size)
    {
        MeshFilter mf = asteroid.AddComponent<MeshFilter>();

        int chunkCount = Random.Range(2, 5);
        GameObject[] chunks = new GameObject[chunkCount];
        CombineInstance[] combines = new CombineInstance[chunkCount];

        for (int i = 0; i < chunkCount; i++)
        {
            chunks[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chunks[i].transform.SetParent(asteroid.transform);
            chunks[i].transform.localPosition = Random.insideUnitSphere * size * 0.15f;
            chunks[i].transform.localScale = Vector3.one * Random.Range(size * 0.5f, size * 0.9f);
            chunks[i].transform.localRotation = Random.rotation;

            MeshFilter chunkMF = chunks[i].GetComponent<MeshFilter>();
            Mesh chunkMesh = chunkMF.mesh;
            DistortMesh(chunkMesh, size * 0.25f);
            chunkMF.mesh = chunkMesh;

            chunks[i].GetComponent<MeshRenderer>().enabled = false;

            combines[i].mesh = chunkMF.sharedMesh;
            combines[i].transform = chunks[i].transform.localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combines, true, true);
        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();
        mf.mesh = combinedMesh;

        asteroid.transform.localScale = Vector3.one;

        foreach (var chunk in chunks)
            Destroy(chunk);
    }

    private void DistortMesh(Mesh mesh, float amount)
    {
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            float noise = Mathf.PerlinNoise(
                vertices[i].x * 2f + Random.Range(0f, 100f),
                vertices[i].y * 2f + Random.Range(0f, 100f)
            );
            Vector3 offset = Random.insideUnitSphere * amount * (0.5f + noise);
            vertices[i] += offset;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private void OnDestroy()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child == null)
                continue;

            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null && mr.material != null)
                Destroy(mr.material);

            Destroy(child.gameObject);
        }
    }
}

public class AsteroidMover : MonoBehaviour
{
    private Vector3 moveDirection;
    private Vector3 rotAxis;
    private float rotSpeed;
    private float wobbleStr;
    private float wobbleSpd;
    private float wobbleOffset;
    private float damage;

    public void Initialize(Vector3 moveDir, float rotationSpeed, float wobbleStrength, float wobbleSpeed, float dmg)
    {
        moveDirection = moveDir;
        rotAxis = Random.onUnitSphere;
        rotSpeed = rotationSpeed;
        wobbleStr = wobbleStrength;
        wobbleSpd = wobbleSpeed;
        wobbleOffset = Random.Range(0f, 100f);
        damage = dmg;
    }

    private void Update()
    {
        float wobble = Mathf.Sin(Time.time * wobbleSpd + wobbleOffset) * wobbleStr;
        Vector3 wobbleDir = Vector3.Cross(moveDirection, Vector3.up).normalized * wobble;

        transform.position += (moveDirection + wobbleDir) * Time.deltaTime;
        transform.Rotate(rotAxis, rotSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }
}
