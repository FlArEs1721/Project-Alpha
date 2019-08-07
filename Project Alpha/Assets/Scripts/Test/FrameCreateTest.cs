using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCreateTest : MonoBehaviour
{
    public GameObject gameFramePrefab;

    private void Start()
    {
        StartCoroutine(TestCor());
    }

    private IEnumerator TestCor()
    {
        yield return new WaitForSeconds(1);

        GameFrame testFrame = Instantiate(gameFramePrefab).GetComponent<GameFrame>();
        testFrame.CreateFrame(Vector2.zero, 0, new Vector2(500, 500), 100, 0.7f, LerpType.SmoothOut);
        testFrame.CreateNote(NoteType.Touch, 0);

        yield return new WaitForSeconds(3);

        testFrame.MoveFrame(new Vector2(200, 0), 1.5f, LerpType.Smooth);
        testFrame.CreateNote(NoteType.Touch, 0);

        yield return new WaitForSeconds(3);

        testFrame.RotateFrame(90, 1.5f, LerpType.Smooth);
        testFrame.CreateNote(NoteType.Touch, 0);

        yield return new WaitForSeconds(3);

        testFrame.RotateFrame(0, 1.5f, LerpType.Smooth);
        testFrame.CreateNote(NoteType.Touch, 0);

        yield return new WaitForSeconds(3);

        testFrame.ResizeFrame(new Vector2(300, 300), 1.5f, LerpType.Smooth);
        testFrame.CreateNote(NoteType.Touch, 0);

        yield return new WaitForSeconds(3);

        GameFrame testFrame2 = Instantiate(gameFramePrefab).GetComponent<GameFrame>();
        testFrame2.CreateFrame(new Vector2(-300, 0), 0, new Vector2(100, 100), 50, 0.7f, LerpType.SmoothOut);
        testFrame.CreateNote(NoteType.Touch, 0);
        testFrame2.CreateNote(NoteType.Touch, 0);

        yield return new WaitForSeconds(3);

        testFrame2.ResizeFrame(new Vector2(100, 500), 1.5f, LerpType.Smooth);
        testFrame.CreateNote(NoteType.Touch, 0);
        testFrame2.CreateNote(NoteType.Touch, 0);
    }
}
