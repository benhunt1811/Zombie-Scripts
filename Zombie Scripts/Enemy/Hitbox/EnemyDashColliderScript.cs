using UnityEngine;
//using UnityEngine.ProBuilder.Shapes;

public class EnemyDashColliderScript : MonoBehaviour, IDashTarget
{
    
    [Header("Enemy Components")]
    private EnemyAnimatorScript enemyAnimator;
    private Enemy enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyAnimator = GetComponentInParent<EnemyAnimatorScript>();
        enemy = this.transform.root.GetComponent<Enemy>();
    }

    public void OnDashHit()
    {
        enemyAnimator.FullStumble();
    }
}
