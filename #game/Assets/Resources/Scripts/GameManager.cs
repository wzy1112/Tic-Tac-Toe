using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private static GameManager gm;

    public static GameManager getGM
    {
        get
        {
            return gm;
        }
    }

    enum PawnState
    {
        none,O,X
    }

    enum GameResult
    {
        playing,win,lose,tie
    }

    enum Turn
    {
        player,computer
    }

    [SerializeField]
    GameObject pawns, pawn_O, pawn_X, win, lose, tie, restart,gamebase,startgame,player,computer,quitgame,setting;
    PawnState[,] states = new PawnState[3, 3];
    GameResult result = GameResult.playing;
    Turn turn = Turn.player;
    int num = 0;
    

    void Awake()
    {
        gm = this;
    }

    #region button
    public void StartGame()
    {
        startgame.SetActive(false);
        gamebase.SetActive(true);
        RestartGame();
    }
    public void RestartGame()
    {
        states = new PawnState[3, 3];
        result = GameResult.playing;
        PlayerTurn();
        num = 0;
        win.SetActive(false);
        lose.SetActive(false);
        tie.SetActive(false);
        restart.SetActive(false);
        quitgame.SetActive(false);

        int count = pawns.transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            Destroy(pawns.transform.GetChild(i).gameObject);
        }

        gamebase.GetComponent<AudioSource>().Play();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//如果是在unity编译器中
#else
        Application.Quit();//否则在打包文件中
#endif

    }

    public void OpenSettingMenu()
    {
        setting.SetActive(true);
    }
    public void GameButtonClick(int x, int y)
    {
        //Debug.Log("ButtonClick:" + x + " " + y);

        if (result == GameResult.playing && turn == Turn.player)
        {

            if (states[x - 1, y - 1] == PawnState.none)
            {
                CreatPawn(x, y, PawnState.O);
            }
            else
            {
                Debug.Log("Can't put pawn here!");
            }
        }
    }

    #endregion

    void PlayerTurn()
    {
        turn = Turn.player;
        player.GetComponent<Image>().color = Color.white;
        computer.GetComponent<Image>().color = Color.gray;
    }

    void CPTTurn()
    {
        turn = Turn.computer;
        computer.GetComponent<Image>().color = Color.white;
        player.GetComponent<Image>().color = Color.gray;
        CPTPlay();
    }

    void GameCheck()
    {
        result = isWin(states,turn);
        switch (result) {
            case GameResult.playing:                
                if (turn == Turn.player)
                    CPTTurn();
                else
                    PlayerTurn();
                break;
            case GameResult.win:
                Debug.Log("You Win!");
                win.SetActive(true);
                restart.SetActive(true);
                quitgame.SetActive(true);
                break;
            case GameResult.lose:
                Debug.Log("You Lose!");
                lose.SetActive(true);
                restart.SetActive(true);
                quitgame.SetActive(true);
                break;
            case GameResult.tie:
                Debug.Log("No one win...");
                tie.SetActive(true);
                restart.SetActive(true);
                quitgame.SetActive(true);
                break;
        }
    }

    GameResult isWin(PawnState[,] _states,Turn _turn)
    {
        bool gameend = false;
        bool win = false;
        for (int i = 0; i < 3; i++) 
        {
            if (_states[0, i] != PawnState.none && _states[0, i] == _states[1, i] && _states[1, i] == _states[2, i])
            { win = true; gameend = true; }
        }
        for (int i = 0; i < 3; i++)
        {
            if (_states[i, 0] != PawnState.none && _states[i, 0] == _states[i, 1] && _states[i, 1] == _states[i, 2])
            { win = true; gameend = true; }
        }
        if (_states[0, 0] != PawnState.none && _states[0, 0] == _states[1, 1] && _states[1, 1] == _states[2, 2])
        { win = true; gameend = true; }
        else if (_states[2, 0] != PawnState.none && _states[2, 0] == _states[1, 1] && _states[1, 1] == _states[0, 2])
        { win = true; gameend = true; }
        else if (num == 9)
            gameend = true;

        if (gameend == true && win == false)
            return GameResult.tie;
        else if (gameend == true && win == true)
        {
            if (_turn == Turn.player)
                return GameResult.win;
            else return GameResult.lose;
        }
        else
            return GameResult.playing;

    }

 

    void CPTPlay()
    {
        int x, y;

        //preview if cpt will win
        for (x = 0; x < 3; x++)
        {
            for(y = 0; y < 3; y++)
            {
                if (states[x, y] == PawnState.none)
                {
                    PawnState[,] pre_states = new PawnState[3, 3];
                    System.Array.Copy(states, pre_states, states.Length);
                    pre_states[x, y] = PawnState.X;
                    if (isWin(pre_states, Turn.computer) == GameResult.win)
                    {
                        StartCoroutine(CPTCreatPawn(x + 1, y + 1));
                        return;
                    }
                }
            }
        }

        //preview if player will win
        for (x = 0; x < 3; x++)
        {
            for (y = 0; y < 3; y++)
            {
                if (states[x, y] == PawnState.none)
                {
                    PawnState[,] pre_states = new PawnState[3, 3];
                    System.Array.Copy(states, pre_states, states.Length);
                    pre_states[x, y] = PawnState.O;
                    if (isWin(pre_states, Turn.player) == GameResult.win)
                    {
                        StartCoroutine(CPTCreatPawn(x + 1, y + 1));
                        return;
                    }
                }
            }
        }

        //random
        bool finish = false;
        while(!finish)
        {
            x = Random.Range(1, 4);
            y = Random.Range(1, 4);
            if (states[x - 1, y - 1] == PawnState.none)
            {
                StartCoroutine(CPTCreatPawn(x, y));
                finish = true;
            }
        }
    }

    private IEnumerator CPTCreatPawn(int x, int y)
    {
        float t = Random.Range(-0.3f, 0.3f);
        yield return new WaitForSeconds(1 + t);
        CreatPawn(x, y, PawnState.X);
    }

    void CreatPawn(int x, int y, PawnState pawnState)
    {
        GameObject pawn;
        if (pawnState==PawnState.O)
        {
            pawn = Instantiate(pawn_O);
            states[x - 1, y - 1] = PawnState.O;
        }
        else
        {
            pawn = Instantiate(pawn_X);
            states[x - 1, y - 1] = PawnState.X;
        }
        pawn.transform.SetParent(pawns.transform);
        pawn.GetComponent<RectTransform>().anchoredPosition= new Vector2(x * 165 - 165 * 2, 165 * 2 - y * 165);
        Debug.Log("states:" + x + " " + y + " " + states[x - 1, y - 1]);
        num++;
        pawns.GetComponent<AudioSource>().Play();
        GameCheck();
    }
}
