using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehaviours/MouseFollower")]
public class MouseFollower : AIBehaviour
{
    public override void Init(GameObject dono, SnakeMovement movimentoDono)
    {
        base.Init(dono, movimentoDono);
    }

    public override void Execute()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3 direction = mouseWorldPos - owner.transform.position;
        direction.z = 0f;

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, ownerMovement.speed * Time.deltaTime);

        owner.transform.position = Vector2.MoveTowards(owner.transform.position, mouseWorldPos, ownerMovement.speed * Time.deltaTime);
    }
}
