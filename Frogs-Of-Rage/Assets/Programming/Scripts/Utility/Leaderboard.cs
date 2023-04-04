using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] [Tooltip("Text elements in order of first to tenth for displaying scores")] private Text[] _scoreTexts;
    [SerializeField] [Tooltip("Text elements in order of first to tenth for displaying names")]  private Text[] _nameTexts;

    [SerializeField] [Tooltip("Input field UI element thats used for players to input their 3 letter name when getting a high score")] private InputField _nameInputField;
    [SerializeField] [Tooltip("The Button the Player Presses to enter their name")] private Button _nameInputConfirmationButton;

    private List<char> _characters = new List<char>(); //list used for storage when adjusting inputs to fit the format

    //Order of operations notes:

    //1. Check if the players score places (CheckScorePlaces) X

    //If it does:
    //2. Pop up the name inputter X
    //3. Take inputed name and score, SendScoreToLeaderboardData X
    //4. Hide/remove name Input field X
    //5. DisplaySavedScoreData X

    //If it does not: 
    //2. DisplauSavedScoreData X

    private void OnEnable() //used to start the leaderboard flow, may be changed to be started elsewhere later
    {
        NewLeaderboardTime(ReceiveCurrentGameTimerTime());
    }

    private void NewLeaderboardTime(float newTime)
    {
        if (CheckScorePlaces(newTime))
        {
            //time places, let player input name
            ToggleNameInputs(true);
        }
        else
        {
            //time dosent place, simply display leaderboard

            ToggleNameInputs(false);
            DisplaySavedScoreData();
        }
    } // ran in OnEnable. begins the flow of the leaderboard. checks if the time is valid, and begins the appropiate stems

    private bool CheckScorePlaces(float time)
    {
        if (LeaderboardData.CheckIfScorePlaces(time))
            return true;
        else
            return false;          
    } //checks if a score places on the leaderboard

    private void SendScoreToLeaderboardData(float newTime, string newName)
    {
        LeaderboardData.AppendLeaderboardData(newTime, newName);
    }  //sends a score and to leaderboard data saved on LeaderboardData

    private void DisplaySavedScoreData()
    {
        List<LeaderboardScoreData> retreivedScoreData = LeaderboardData.GetSavedScoreData();

        if(_scoreTexts == null || _nameTexts == null)
        {
            Debug.LogError("No score texts or name texts set on Leaderboard on <b>" + gameObject.name + "</b> Please set the fields in the inspector");
            return;
        } //saftey check for null

        if(_scoreTexts.Length < retreivedScoreData.Count)
        {
            Debug.LogWarning("Score text array on LeaderBoard <b>" + gameObject.name + "</b> was to short to account for all of the leaderboard data, there is <b>" + retreivedScoreData.Count + "</b> trying to be displayed but only </b>" + _scoreTexts.Length + ("</b> available for display. <b>The list has been clamped to the current length of the available text elements</b>"));
            retreivedScoreData = UtilityFunctions.ClampListLength(retreivedScoreData, _scoreTexts.Length);
        } //saftey check for available score texts

        if(_nameTexts.Length < retreivedScoreData.Count)
        {
            Debug.LogWarning("Name text array on LeaderBoard <b>" + gameObject.name + "</b> was to short to account for all of the leaderboard data, there is <b>" + retreivedScoreData.Count + "</b> trying to be displayed but only </b>" + _nameTexts.Length + ("</b> available for display. <b>The list has been clamped to the current length of the available text elements</b>"));
            retreivedScoreData = UtilityFunctions.ClampListLength(retreivedScoreData, _nameTexts.Length);
        } //saftey check for available name texts

        //after those checks, if the function is still being ran, the text element arrays cannot be shorter than the retreived data array from the checks. they will be longer than or equal in length to retreivedScoreData

        //updates the score texts
        for (int i = 0; i < retreivedScoreData.Count; i++)
        {
            _scoreTexts[i].text = UtilityFunctions.ConvertSecondsToStandardTimeFormatString(retreivedScoreData[i].time);
            _nameTexts[i].text = retreivedScoreData[i].name;
        }
    } //displays the scores saved on LeaderboardData on the leaderboard

    private float ReceiveCurrentGameTimerTime()
    {
        if (GameManager.Instance && GameManager.Instance.gameTimer)
        {
            return GameManager.Instance.gameTimer.totalTime;
        }
        else
        {
            Debug.LogError("Game timer Instance could not be gotten from the game manager, returning a default time of 150");
            return 150;
        }

    } //receives current time from the game managers game timer. null checks and returns a default value (150) if the instance is null of either the timer or game manager

    private void ToggleNameInputs(bool state)
    {
        if(_nameInputField && _nameInputConfirmationButton)
        {
            if(state)
            {
                //clears text for new input
                _nameInputField.text = "";
            }

            _nameInputField.gameObject.SetActive(state);
            _nameInputConfirmationButton.gameObject.SetActive(state);
        }
        else
        {
            Debug.LogWarning("No NameInputField or NameInputConfirmationButton assigend to Leaderboard on object: <b>" + gameObject.name + "</b> Please attatch one in the inspector");
        }
    } // Toggles the name input field and button based off an inputted state



    public void AdjustInputs()
    {
        if (_nameInputField)
        {
            _characters.Clear();

            for (int i = 0; i < 3; i++)
            {
                if (_nameInputField.text.Length > i)
                {
                    if (char.IsLetter(_nameInputField.text[i]))
                    {
                        _characters.Add(char.ToUpper(_nameInputField.text[i]));
                    }
                }
            }

            string returnString = "";
            for (int i = 0; i < _characters.Count; i++)
            {
                returnString += _characters[i];
            }

            _nameInputField.text = returnString;

        }
        else
        {
            Debug.LogWarning("No NameInputField assigend to Leaderboard on object: <b>" + gameObject.name + "</b> Please attatch one in the inspector");
        }
    } //adjusts inputs made on the name inputter to fit format. run in the input field whenever a value is changed

    public void ReceiveInputedName()
    {
        if (_nameInputField)
        {
            if(_nameInputField.text.Length > 0)
            {
                SendScoreToLeaderboardData(ReceiveCurrentGameTimerTime(), _nameInputField.text);
                ToggleNameInputs(false);
                DisplaySavedScoreData();
            }
        }
        else
        {
            Debug.LogWarning("No NameInputField assigend to Leaderboard on object: <b>" + gameObject.name + "</b> Please attatch one in the inspector");
        }
    } //reveives the name inputed into the name input box. disables the input fields, sends it to LeaderboardData and Displays times if a valid name is inputted
}
