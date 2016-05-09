using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {

	public void LoadNextLevel()
	{
		StartCoroutine(LoadNextLevelIE());
	}

	public IEnumerator LoadNextLevelIE()
	{
		float FadingTime = GameObject.Find("Loader").GetComponent<SceneFading>().StartFade(1);
		yield return new WaitForSeconds(FadingTime);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void LoadSpecialLevel(string name)
	{
		StartCoroutine(LoadSpecialLevelIE(name));
	}

	public IEnumerator LoadSpecialLevelIE(string name)
	{
		float FadingTime = GameObject.Find("Loader").GetComponent<SceneFading>().StartFade(1);
		yield return new WaitForSeconds(FadingTime);
		SceneManager.LoadScene(name);
	}
}
