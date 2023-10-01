using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DedicatedStudy_Server;

public class Player
{
    public int id;
    public string username;

    public Vector3 position;
    public Quaternion rotation;

    private float moveSpeed = 3f / Constants.TICKS_PER_SEC;
    private float horizontal = 0f;
    private float vertical = 0f;

    public Player(int _id, string _username, Vector3 _spawnPosition)
    {
        id = _id;
        username = _username;
        position = _spawnPosition;
        rotation = Quaternion.Identity;
    }
    public void Update()
    {
        Vector2 _inputDir = new Vector2(horizontal, vertical);
        Move(_inputDir);
    }
    private void Move(Vector2 _inputDir)
    {
        //정면 방향
        Vector3 _forward = Vector3.Transform(new Vector3(0, 0, 1), rotation);
        //백터곱으로 우측 방향 구함
        Vector3 _right = Vector3.Normalize(Vector3.Cross(_forward, new Vector3(0, -1, 0)));

        Vector3 _moveDir = _right * _inputDir.X + _forward * _inputDir.Y;
        position += _moveDir * moveSpeed;

        //굳이 패킷을 나눠서 보내는 이유는..
        //플레이어 위치는 모든 클라이언트에서 변경되어야하지만,
        //플레이어의 방향값은 본인을 제외하고 다른 클라들에서만 회전 값이 반영되야함. 
        //우리가 애초에 이동할떄 방향값은 그대로 받아옥고 있어서.. 본인의 회전은 로컬에서 본인이 돌리고 있기때문...
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }
    public void SetInput(float _horizontal, float _vertical, Quaternion _rotation)
    {
        horizontal = _horizontal;
        vertical = _vertical;
        rotation = _rotation;
    }
}