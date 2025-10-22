using UnityEngine;

public class EnemyAnimController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void SetIsDead(bool isDead)
    {
        _animator.SetBool(ParamConsts.IS_DEAD, isDead);
    }
}
