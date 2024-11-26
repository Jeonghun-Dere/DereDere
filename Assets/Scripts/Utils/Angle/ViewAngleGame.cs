using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Entity.ViewAngle {


	public class ViewAngleGame : MonoBehaviour {

		public float viewRadius; // 시야 범위
		
		[Range(0,360)]
		public float viewAngle; // 시야각

		[SerializeField] public LayerMask target; // 봤을때 반응하는 놈
		[SerializeField] public LayerMask obstacle; // 봤을 때 ray가 막히는 놈

		[HideInInspector]
		public List<Transform> visibleTargets = new List<Transform>(); // 레이를 쏘면서 지금 보이는 놈 리스트 넣습니다.

		public Transform pivot;


		void Start() {
			
			StartCoroutine("FindTarget", 0.1f);// 눈을 깜빡이는 시간
			
		}


		IEnumerator FindTarget(float delay) {
			while (true) {
				yield return new WaitForSeconds (delay);
				FindTargets();
			}
		}
		
		

		void FindTargets() {
			visibleTargets.Clear();
			Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius);
			//bool flag = false;
			for (int i = 0; i < targetsInViewRadius.Length; i++)
			{
				if (!targetsInViewRadius[i].CompareTag("Player")) continue;
				print(targetsInViewRadius[i]);
				Transform target = targetsInViewRadius[i].transform;
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				Vector3 direction = (pivot.position - transform.position).normalized;
				// float z = transform.rotation.eulerAngles.z;
				// Vector2 direction = new Vector2(Mathf.Cos(z * Mathf.Deg2Rad), Mathf.Sin(z * Mathf.Deg2Rad));
				// Quaternion q1 = Quaternion.LookRotation(target.position - transform.position);
				// float bodyRotValue = Quaternion.Angle(transform.rotation, q1);
				// if (Vector3.Angle(dirPiv, dirToTarget) < viewAngle/2) {
				if(Vector3.Angle(dirToTarget, direction) < viewAngle/2){
					float dstToTarget = Vector2.Distance (transform.position, target.position);
					if (!Physics2D.Raycast (transform.position, dirToTarget, dstToTarget, obstacle)) {
						visibleTargets.Add (target);
						// Debug.Log("complete");
					}
				}

			}

			
		}
		
		public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
			if (!angleIsGlobal) {
				angleInDegrees -= transform.eulerAngles.z;
			}
			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
		}


		public void GetPivot()
		{
			int countOfChlid = this.gameObject.transform.childCount;
			for (int i = 0; i < countOfChlid; i++)
			{
				GameObject _human = this.gameObject.transform.GetChild(i).gameObject;
				if (_human.name == "piv") pivot = _human.transform;

			}
		}
	

	}
}