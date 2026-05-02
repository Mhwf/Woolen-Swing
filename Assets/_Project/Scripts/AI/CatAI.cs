using UnityEngine;
using UnityEngine.AI;
using WoolenSwing.Management;

namespace WoolenSwing.AI
{
    public class CatAI : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float killDistance = 1.2f;// distance at which the cat will catch the ball
        private NavMeshAgent _agent;

        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            _agent.SetDestination(target.position);

            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= killDistance)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}