using UnityEngine;
using Unity.Netcode;
using Unity.Collections;




public class PlayerNetwork_TPP : NetworkBehaviour
{

    [SerializeField] private GameObject spawnedObjectPrefab;
    Transform spawnedObjectTransform;





    // Network variable init with params to see if a client can write to the network
    // datapoint must be a value class and not a reference class (blue)
    private NetworkVariable<datapoint> network_data_variable = new NetworkVariable<datapoint>(
        new datapoint
        {
            _number = 56,
            _boolean = true,
            _message = "hello world",
        },
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner
    );
    // When creating a custom data type to place in the networkVariable's generic... need to extend struct from INetworkSerializable
    public struct datapoint : INetworkSerializable
    {
        public int _number;
        public bool _boolean;
        public FixedString128Bytes _message;   // strings are ref types thus you need to use `fixedstrings`

        // Must implement this function if extending class from INetworkSerializable (just add the attribs from the struct as shown)
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _number);
            serializer.SerializeValue(ref _boolean);
            serializer.SerializeValue(ref _message);
        }
    }









    void Update()
    {
        // check if input is coming from the player object-NetworkBehaviour (dont control anyone else...)
        if (!IsOwner) return;





        // Sync variables using RPC (test)
        if (Input.GetKey(KeyCode.Tab)) testServerRpc();





        //Spawning network Objects
        if (Input.GetKeyDown(KeyCode.O))
        {
            spawnedObjectTransform = Instantiate(spawnedObjectPrefab.transform);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true);
            Destroy(spawnedObjectTransform);
        }






        // // MOVE PLAYER
        // Vector3 moveDir = new Vector3(0, 0, 0);
        // if (Input.GetKey(KeyCode.Space)) moveDir.y = +1f;
        // if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        // if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        // if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;
        // if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        // float moveSpeed = 3f;
        // transform.position += moveDir * moveSpeed * Time.deltaTime;


    }




    // This function , activated on client will RUN ON SERVER
    [ServerRpc]
    private void testServerRpc()
    {
        Debug.Log("Broadcasting from client to server ClientID:" + OwnerClientId);
    }




    // This function , activated on server will RUN ON CLIENT
    [ClientRpc]
    private void testClientRpc()
    {
        Debug.Log("Broadcasting from server to client ClientID:" + OwnerClientId);
    }




}
