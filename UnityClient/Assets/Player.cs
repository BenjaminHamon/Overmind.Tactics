using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class Player : MonoBehaviour
	{
		[SerializeField]
		private Camera worldCamera;

		[SerializeField]
		private Character selection;

		[SerializeField]
		private GameObject selectionIndicator;

		public void Update()
		{
			if (Input.GetMouseButtonUp(0))
			{
				Vector2 hitOrigin = worldCamera.ScreenToWorldPoint(Input.mousePosition);
				RaycastHit2D hit = Physics2D.Raycast(hitOrigin, Vector2.zero);
				Select(hit.collider != null ? hit.collider.GetComponent<Character>() : null);
			}

			if (Input.GetMouseButtonUp(1) && (selection != null))
			{
				Vector2 hitOrigin = worldCamera.ScreenToWorldPoint(Input.mousePosition);
				RaycastHit2D hit = Physics2D.Raycast(hitOrigin, Vector2.zero);
				if (hit.collider != null)
				{
					Character target = hit.collider.GetComponent<Character>();
					if (target == null)
					{
						Vector3 newPosition = selection.transform.localPosition;
						newPosition.x = Mathf.Round(hitOrigin.x);
						newPosition.y = Mathf.Round(hitOrigin.y);
						selection.transform.localPosition = newPosition;
					}
				}
			}
		}

		public void Select(Character newSelection)
		{
			selection = newSelection;

			if (selection == null)
			{
				selectionIndicator.transform.SetParent(transform, false);
				selectionIndicator.SetActive(false);
			}
			else
			{
				selectionIndicator.transform.SetParent(selection.transform, false);
				selectionIndicator.SetActive(true);
			}
		}
	}
}
