using UnityEngine;

public class Leg : MonoBehaviour
{
    //可编辑
    [SerializeField] public Transform leg; 
    [SerializeField] private Transform foot; 
    [SerializeField] private Transform legRoot;    
    [SerializeField] private Transform IK_Target;    
    [SerializeField] private float length;
    [SerializeField] private Leg otherLeg;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private LayerMask layer;

    //内部字段
    private float legX;
    private float legY;
    private bool isLerp;
    private Vector2 target;
    private float shmTime;

    //公共字段
    [HideInInspector] public bool Active;

    private void Update()
    {
        //腿的弧度
        float x_adj = legRoot.position.x - legX;
        float y_opp = legRoot.position.y - legY;
        float legRad = Mathf.Atan2(y_opp, x_adj);
        //脚的弧度
        float tempX = legRoot.position.x - Mathf.Cos(legRad) * length;
        float tempY = legRoot.position.y - Mathf.Sin(legRad) * length;
        x_adj = tempX - IK_Target.position.x;
        y_opp = tempY - IK_Target.position.y;
        float footRad = Mathf.Atan2(y_opp, x_adj);
        //腿的位置
        legX = IK_Target.position.x + Mathf.Cos(footRad) * length;
        legY = IK_Target.position.y + Mathf.Sin(footRad) * length;
        //最终定位
        leg.SetPositionAndRotation(new Vector2(legX, legY), Quaternion.Euler(new Vector3(0, 0, legRad * Mathf.Rad2Deg)));
        foot.SetPositionAndRotation(new Vector2(IK_Target.position.x, IK_Target.position.y), Quaternion.Euler(new Vector3(0, 0, footRad * Mathf.Rad2Deg)));

        //射线,寻找蜘蛛的下一次落脚点
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, -rayOrigin.up, 5, layer);
        if (hit)
        {
            target = hit.point;
            Debug.DrawRay(rayOrigin.position, rayOrigin.up * (hit.point - (Vector2)rayOrigin.position) - new Vector2(0, 0.1f), Color.red, 0.05f);
        }

        //根据条件来控制蜘蛛的步伐
        if (Mathf.Abs(target.x - IK_Target.position.x) > 0.7f) { isLerp = true; }
        if (isLerp && Active) 
        { 
            IK_Target.position = Vector2.MoveTowards(IK_Target.position, target + new Vector2(0.1f, 0), Time.deltaTime * 3);
            shmTime += Time.deltaTime;
            IK_Target.position += new Vector3(0, Mathf.Cos(shmTime) * 1.5f) * Time.deltaTime; //简谐运动做为蜘蛛抬腿运动的数学模型
        }
        if (Vector2.Distance(IK_Target.position, target + new Vector2(0.1f, 0)) < 0.1f)
        {
            isLerp = false;
            shmTime = 0;
            //协调蜘蛛的另一支腿
            Active = false;
            otherLeg.Active = true;
        }
    }
}