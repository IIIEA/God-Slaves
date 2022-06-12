using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
	[SerializeField] MenuButtonController menuButtonController;
	[SerializeField] Animator animator;
	[SerializeField] AnimatorFunctions animatorFunctions;
	[SerializeField] int thisIndex;

	void Update()
	{
		if (menuButtonController.index == thisIndex)
		{
			animator.SetBool("selected", true);

			if (Input.GetAxis("Submit") == 1)
			{
				animator.SetBool("pressed", true);

				switch (menuButtonController.index)
				{
					case 0:
						SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
						break;
					case 1:
						Application.Quit();
						break;
					default:
						break;
				}
			}
			else if (animator.GetBool("pressed"))
			{
				animator.SetBool("pressed", false);
				animatorFunctions.disableOnce = true;
			}
		}
		else
		{
			animator.SetBool("selected", false);
		}
	}
}
