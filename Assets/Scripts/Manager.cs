using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class Manager : MonoBehaviour
{

    #region Variable

    [Header("Config Manager")]

    public InventoryPool pool;

    public Player_UI playerLeftScreen;
    public Player_UI playerRightScreen;

    public Color selectedSpeedColor;
    public Color unselectedSpeedColor;

    private byte[] layers = { 16, 8, 8, 5};

    public TextMeshProUGUI[] speedButtons;
    public TextMeshProUGUI[] statsText;


    public bool saveBestOne;
    public bool loadBestOne;
    public string bestOneLoadName;
    public string bestOneSaveName;

    [Header("Config Simulation")]

    [Range(2, 200)] public int populationSize;

    [Range(1, 50)] public int roundMax;

    [Range(0.0001f, 1f)] public float mutationChance;

    [Range(0f, 1f)] public float mutationStrength;

    [Range(1, 100000)] public int generationsJump;
    public bool jumpGeneration;

    public List<Player> players = new List<Player>();


    

    private int lastPlayerId = 0;   //The index for creation of new player

    private int genNumber = 0;  //Generation Number. (Start at 1).
    private bool generationIsFinish = false;

    private int batchNumber = 0;
    private bool batchIsFinish = false;

    private int[] playerIds = new int[2];
    private int roundNumber = 0; //The Round Number 0 is the start round, the first action is in the round number 1
    private int battleNumber = 0; //The First Battle start at 1
    private bool fightIsFinish = true;


    private int simulationSpeed = 0;
    private int counter = 0;

    #endregion

    void Start()
    {
        changeSpeed(0);
        CreateFirstGeneration();
        InitNewGeneration(true);
    }

    public void Update()
    {
        updateStatsCounters(0, genNumber);
        updateStatsCounters(1, batchNumber);
        updateStatsCounters(2, battleNumber);
        updateStatsCounters(3, roundNumber);
        if (jumpGeneration && simulationSpeed == 0)
        {
            jumpGeneration = false;
            JumpGenerations(generationsJump);
        }
    }

    public void FixedUpdate()
    {
        if (simulationSpeed != 0)
        {
            
            if (counter == 10) // On va donc simuler le combat toutes les 200ms, 40ms, 20ms. Pour 1x, 5x, 10x. 
            {
                counter = 0;
                SimulateFight();
            }
            else
            {
                counter++;
            }
        }
    }


  #region Init
    /*
    Initialization function executed at the beginning of the program.

    CreateFirstGeneration : Create the first Generation of Players.
    */
    private void CreateFirstGeneration()
    {
        for (int i = 0; i < populationSize; i++)
        {

            lastPlayerId++;
            Player player = new Player(lastPlayerId,new NeuronNetwork(layers));

            //Select the player's inventory
            int indexInv = (int)UnityEngine.Random.Range(0, pool.inventories.Count);
            player.NewGeneration( pool.inventories[indexInv], indexInv, pool.inventories.Count);

            if (loadBestOne)
            {
                player.brain.Load("SavedBrains/" + bestOneLoadName + ".txt");

                if (i != 0) //Only the first one has the copy, others have a modified version of it.
                    player.brain.Mutate(mutationChance, mutationStrength);
            }
            players.Add(player);
        }

        Debug.Log("First Gen Created");
    }

  #endregion

  #region Utils
    /*
    MakeRound is the main function, it's make the next move and move to the next stage of the fight.
    */

    #region Round
    private void MakeRound(bool log)
    {
        roundNumber++;

        //int idPlayerA = playersOrder[currentPlayerIds[0]];
        //int idPlayerB = playersOrder[currentPlayerIds[1]];

        Player playerA = players[playerIds[0]];
        Player playerB = players[playerIds[1]];

        playerA.chooseAction((float)roundNumber / (float)roundMax);
        playerB.chooseAction((float)roundNumber / (float)roundMax);

        playerA.DoAction();
        playerB.DoAction();

        //Tests for the end of the fight
        if (roundNumber == roundMax)
        {
            /*
            if (playerA.health > playerB.health)
                PlayerWin(true, playerA, playerB, log);

            else if (playerB.health > playerA.health)
                PlayerWin(true, playerB, playerA, log);
            */
            //If Nobody Win
            PlayerWin(false, playerA, playerB, log);
        }
        else if (playerA.isDead)
        {
            PlayerWin(true, playerB, playerA, log);
        }
        else if (playerB.isDead)
        {

            PlayerWin(true, playerA, playerB, log);
        }
    }

    private void PlayerWin(bool isWin, Player A, Player B, bool log)
    {
        if (isWin)
        {
            A.win = true;
        }

        A.EndFight();
        B.EndFight();
        fightIsFinish = true;
        if (log)
        {
            ShowFinalFight();
        }

    }

    #endregion

    #region Fight
    private void InitNewFight(bool log)
    {
        if (!batchIsFinish)
        {
            fightIsFinish = false;
            battleNumber++;
            roundNumber = 0;
            playerIds[0] += 1;
            playerIds[1] = (playerIds[0] + batchNumber ) % populationSize;

            //int idPlayerA = playersOrder[currentPlayerIds[0]];
            //int idPlayerB = playersOrder[currentPlayerIds[1]];

            Player playerA = players[playerIds[0]];
            Player playerB = players[playerIds[1]];

            playerA.NewFight(playerB);
            playerB.NewFight(playerA);

            playerLeftScreen.setPlayer(playerA);
            playerRightScreen.setPlayer(playerB);
            if (log)
            {
                Debug.Log(" ");
                Debug.Log("// Gen " + genNumber + " - Batch " + batchNumber + " - Fight " + battleNumber + "//");
                Debug.Log("Opposants : N°" + players[playerIds[0]].id + " and N°" + players[playerIds[1]].id);
            }
        }
    }

    private void ShowFinalFight()
    {
        //int idPlayerA = playersOrder[playerIds[0]];
        //int idPlayerB = playersOrder[playerIds[1]];

        string winnerText = "";

        if (players[playerIds[0]].win)
        {
            winnerText = "N° " + players[playerIds[0]].id + " Win.";
        }
        else if (players[playerIds[1]].win)
        {
            winnerText = "N° " + players[playerIds[1]].id + " Win.";
        }
        else
        {
            winnerText = "No Winner";
        }
        Debug.Log(winnerText);
        Debug.Log("Scores : N°" + players[playerIds[0]].id + " = " + players[playerIds[0]].score + " / N°" + players[playerIds[1]].id + " = " + players[playerIds[1]].score);

    }

    #endregion

    #region FightBatch

    private void InitNewBatch(bool log)
    {
        batchNumber++;
        //playersOrder = playersOrder.OrderBy(i => UnityEngine.Random.value).ToList();

        battleNumber = 0;
        roundNumber = 0;

        playerIds[0] = -1;
        playerIds[1] = -1;

        batchIsFinish = false;

        if (log)
            Debug.Log("// Batch " + batchNumber + " //");
    }

    private void CloseLastBatch(bool log)
    {
        int scoreMoy = 0;
        for (int i = 0; i < populationSize; i++)
            scoreMoy += players[i].score;
        scoreMoy /= populationSize;

        if (log)
        {
            Debug.Log(" ");
            Debug.Log("// Gen " + genNumber + " - Batch " + batchNumber + "// End Result");
            Debug.Log("- Score Moyen : " + scoreMoy);
        }
    }
    #endregion

    #region Generation

    private void InitNewGeneration(bool log)
    {
        genNumber++;
        batchNumber = 0;

        if (log)
            Debug.Log("// Gen " + genNumber + " //");

        generationIsFinish = false;

        InitNewBatch(log);
    }
  
    private void CloseLastGeneration(bool log)
    {
        players.Sort();

        int n = populationSize;
        int victoryMoy = 0, scoreMoy = 0, minGames = 1000, maxGames = -1, moyGames = 0;
        for (int i = 0; i < populationSize; i++)
        {
            victoryMoy += players[i].tvictoryPoints;
            scoreMoy += players[i].score;
            moyGames += players[i].totalFight;
            if ( minGames > players[i].totalFight)
                minGames = players[i].totalFight;

            if (maxGames < players[i].totalFight)
                maxGames = players[i].totalFight;
        }
        moyGames /= populationSize;
        victoryMoy /= populationSize; 
        scoreMoy /= populationSize;

        if (log)
        {
            Debug.Log(" ");
            Debug.Log("// Gen " + genNumber + " // End Result");
            Debug.Log("Top 5 : ");
            Debug.Log("- N°" + players[n - 1].id + " : " + players[n - 1].tvictoryPoints + "     (Gen " + players[n - 1].gen + ")");
            Debug.Log("- N°" + players[n - 2].id + " : " + players[n - 2].tvictoryPoints + "     (Gen " + players[n - 2].gen + ")");
            Debug.Log("- N°" + players[n - 3].id + " : " + players[n - 3].tvictoryPoints + "     (Gen " + players[n - 3].gen + ")");
            Debug.Log("- N°" + players[n - 4].id + " : " + players[n - 4].tvictoryPoints + "     (Gen " + players[n - 4].gen + ")");
            Debug.Log("- N°" + players[n - 5].id + " : " + players[n - 5].tvictoryPoints + "     (Gen " + players[n - 5].gen + ")");
            Debug.Log("Stats : ");
            Debug.Log("- Score Moyen : " + scoreMoy + " - Points Moyen : " + victoryMoy + " - Min/Moy/Max Games :" + minGames + "/" + moyGames + "/" + maxGames);
        }

        if ( saveBestOne)
            players[n - 1].brain.Save("SavedBrains/" + bestOneSaveName + ".txt");

        n = populationSize / 2;
        for (int i = 0; i < n; i++)
        {
            lastPlayerId++;
            players[i] = players[n + i].Copy(players[i], lastPlayerId, genNumber + 1);
            players[i].brain.Mutate(1/mutationChance, mutationStrength);
        }

        int indexInv;
        for (int i = 0; i < populationSize; i++)
        {
            indexInv = (int)UnityEngine.Random.Range(0, pool.inventories.Count);
            players[i].NewGeneration(pool.inventories[indexInv], indexInv, pool.inventories.Count);

        }


    }
    #endregion


    #endregion

    #region Manual
    /*
    Manual function used by buttons or Others Manuals Functions.

    The log parameter is used to display the result of the Round/Fight/Gen into the console.
    */
    public void NextRound(bool log)
    {
        //Detect if we are at the beggining of a Batch without a fight selected
        if (playerIds[0] == -1)
            InitNewFight(log);

        else if (!fightIsFinish) //Execute the Round Function if we are into the fight
            MakeRound(log);
    }

    public void NextFight(bool log)
    {
        //Detect if we are at the beggining of a Batch without a fight selected
        if (playerIds[0] == -1)
            InitNewFight(log);

        else if (!batchIsFinish)
        {
            if (fightIsFinish)
            {
                batchIsFinish = 
                        (battleNumber == populationSize)
                    ||  ( (populationSize % 2 == 0)&&(batchNumber == populationSize / 2)&&( battleNumber == populationSize/2 ) );

                if (!batchIsFinish)
                    InitNewFight(log);

            }
            else
            {
                while (!fightIsFinish)
                {
                    NextRound(false);
                }
                if (log)
                    ShowFinalFight();
            }

        }
    }

    public void NextBatch(bool log)
    {
        //If the current Generation is finish
        if (batchIsFinish)
        {
            generationIsFinish =
            ((populationSize % 2 == 0) && (batchNumber >= populationSize / 2))
            || ((populationSize % 2 == 1) && (batchNumber >= (populationSize - 1) / 2));

            if (generationIsFinish && log)
                CloseLastGeneration(log);
            else if (!generationIsFinish)
                InitNewBatch(log);
        }
        else
        {
            while (!batchIsFinish)
                NextFight(false);
            //When we press the nextBatch button we don't want to have a list of all the Fight.
            CloseLastBatch(log);
        }
    }

    public void NextGeneration(bool log)
    {
        //If the current Generation is finish
        if (generationIsFinish)
            InitNewGeneration(log);
        else
        {
            while (!generationIsFinish)
            {
                //When we press the nextGeneration button we don't want to have a list of all the Batch.
                NextBatch(false);
            }
            CloseLastGeneration(log);
        }
    }
    #endregion

    #region Automatic
    /*
    
    */
    private void JumpGenerations(int jump)
    {
        if (!generationIsFinish)
        {
            NextGeneration(true);
        }
        //On fait 2 fois la valeur de jump car la première fois on créer la génération et la deuxième fois on fait tout les combats de la génération.
        for (int i = 0; i < (jump-1) * 2; i++)
        {
            //On affiche que les centaines de combats
            if (i % 100 == 0)
                NextGeneration(true);
            else
                NextGeneration(false);

        }
        NextGeneration(true);
        NextGeneration(true);
    }
    private void SimulateFight()
    {
        if (playerIds[0] == -1)
        {
            InitNewFight(true);
        }

        else if (!fightIsFinish && !batchIsFinish && !generationIsFinish)
            MakeRound(true);

        else if (fightIsFinish && !batchIsFinish &&!generationIsFinish)
        {
            if (battleNumber == (populationSize / 2))
                batchIsFinish = true;

            else
                InitNewFight(true);
        }
        else if (batchIsFinish && !generationIsFinish)
        {
            if (batchNumber == (populationSize - 1))
            {
                generationIsFinish = true;
                CloseLastGeneration(true);
            }
            else
                InitNewBatch(true);
        }
        else if (generationIsFinish)
            InitNewGeneration(true);
    }
    public void changeSpeed(int i)
    {
        simulationSpeed = i;
        counter = 0;
        switch (i)
        {
            case 0:
                Time.timeScale = 1;
                break;
            case 1:
                Time.timeScale = 1;
                break;
            case 2:
                Time.timeScale = 5;
                break;
            case 3:
                Time.timeScale = 10;
                break;
            default:
                break;
        }
        UpdateSpeedButton();
    }
  #endregion

  #region Visual
    /*
    Modification of the Visual

    UpdateStatsCounters : Update each stats depending on which stats you ask for (_id).

    UpdateSpeedButton : Update the Visual of the SpeedButton
    */
    
    private void updateStatsCounters(int _id, int _value)
    {
        statsText[_id].text = statsText[_id].text.Split(':')[0] + (":\n" + _value);
    }


    private void UpdateSpeedButton()
    {
        for (int _i = 0; _i < speedButtons.Length; _i++)
        {
            if (_i == simulationSpeed) 
                speedButtons[_i].color = selectedSpeedColor;
            else 
                speedButtons[_i].color = unselectedSpeedColor;
        }
    }
  #endregion


}



/* TODO
 * 
 * Round
 * Fight
 * Manual
 * Automatic
 * Visual
 */