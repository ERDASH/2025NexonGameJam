using System.Collections;
using UnityEngine;

public class PlayerFSM : MonoBehaviour
{
    public Animator animator;
    public float backDelaySeconds = 1f;

    private Coroutine backToDefaultCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    IEnumerator ReturnToDefaultAfterDelay()
    {
        yield return new WaitForSeconds(backDelaySeconds);
        ResetAllTriggers();
        animator.SetTrigger("BackDefault");
    }

    void RestartBackToDefaultCoroutine()
    {
        if (backToDefaultCoroutine != null)
            StopCoroutine(backToDefaultCoroutine);

        backToDefaultCoroutine = StartCoroutine(ReturnToDefaultAfterDelay());
    }

    public void ComeLegal()
    {
        ResetAllTriggers();
        animator.SetTrigger("ComeLegal");
       // RestartBackToDefaultCoroutine();
    }

    public void CheckSuccess()
    {
        ResetAllTriggers();
        animator.SetTrigger("CheckSuccess");
        RestartBackToDefaultCoroutine();
    }

    public void CheckFail()
    {
        ResetAllTriggers();
        animator.SetTrigger("CheckFail");
        RestartBackToDefaultCoroutine();
    }

    public void CheckMistake()
    {
        ResetAllTriggers();
        animator.SetTrigger("Mistaking");
        RestartBackToDefaultCoroutine();
    }

    public void SuccessBoom()
    {
        ResetAllTriggers();
        animator.SetTrigger("Success");
        RestartBackToDefaultCoroutine();
    }

    public void HighScore()
    {
        ResetAllTriggers();
        animator.SetTrigger("HighScore");
 //       RestartBackToDefaultCoroutine();
    }

    public void GameOver()
    {
        ResetAllTriggers();
        animator.SetTrigger("GameOver");
   //     RestartBackToDefaultCoroutine();
    }

    public void ResetAllTriggers()
    {
        string[] triggers = {
            "ComeLegal", "CheckSuccess", "CheckFail", "Mistaking",
            "Success", "HighScore", "GameOver", "BackDefault"
        };

        foreach (var trigger in triggers)
            animator.ResetTrigger(trigger);
    }
}