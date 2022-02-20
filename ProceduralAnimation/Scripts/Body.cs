using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] private Leg rightLeg;
    [SerializeField] private Leg leftLeg;
    [SerializeField] private Transform head;
    [SerializeField] private float moveSpeed;

    private void Start()
    {
        rightLeg.Active = true;
        leftLeg.Active = false;
    }

    private void Update()
    {
        //移动
        transform.position += moveSpeed * Time.deltaTime * new Vector3(1, 0, 0);

        //调整头的高度
        float avgHigh = (rightLeg.leg.position.y + leftLeg.leg.position.y) * 0.5f;
        head.position = Vector2.MoveTowards(head.position, new Vector2(head.position.x, avgHigh - 0.2f), Time.deltaTime);
    }
}