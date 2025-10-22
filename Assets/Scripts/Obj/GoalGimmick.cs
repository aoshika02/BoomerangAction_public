using UnityEngine;

public class GoalGimmick : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(ParamConsts.PLAYER_TAG))
        {
            GameManager.Instance.Goal();
        }
    }
}
