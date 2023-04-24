using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR 
using UnityEditor;


public class HatsEditor : EditorWindow
{

    private List<HatData> _loadedList;

    private static EditorWindow _editorInstance;

    private static bool _windowOpen;
    public static bool WindowOpen
    {
        get { return _windowOpen; }
    }

    #region"Editor Display"
    //Gui Styles
    GUIStyle _labelStyle;
    GUIStyle _idStyle;
    GUIStyle _validationTextStyle;
    GUIStyle _databaseDisplayStyle;

    //currentRect
    private Rect _currentHat;

    //Validation Rect;
    private Rect _vallidationRect;

    //AddToDatabase Rect
    private Rect _addToDatabaseRect;

    //current hat fields rect
    private Rect _currentHatfieldsRect;
    //name rects
    private Rect _currentHatNameRect;
    private Rect _currentHatNameTextRect;
    //mesh rects
    private Rect _currentHatMeshRect;
    private Rect _currentHatMeshTextRect;
    //material rects
    private Rect _currentHatMaterialRect;
    private Rect _currentHatMaterialTextRect;
    //Id Rect
    private Rect _iDRect;

    private int _displayScrollIndexing = 0;

    public bool _overwriteMode = false;
    #endregion

    #region "Current Variables (sent to HatData when saved)
    private int _currentID;
    private string _currentName;
    private Mesh _currentMesh;
    private Material _currentMat;
    #endregion

    string _vallidationMessage = "";

    private HatDatabaseRetreiver databaseRetreiver;
    private HatDatabaseWriter databaseWriter;

    //listcolors, for ease of differentiation
    Color listColor1 = new Color(0.2f, 0.2f, 0.2f);
    Color listColor2 = new Color(0.35f, 0.35f, 0.35f);

    // [MenuItem("Frog Editors/Edit Hat List", priority = 0)] Editor Abandoned
    public static void ShowDefaultScoreEditor() //creates editor window when tab is clicked
    {
        _editorInstance = GetWindow<HatsEditor>("Edit Hat List");
        _editorInstance.minSize = new Vector2Int(500, 500);
        _editorInstance.maxSize = new Vector2Int(500, 500);
    }

    private void Awake()
    {
        _windowOpen = true;
        InitilizeCurrentDisplay();
        LoadEditorFromDatabase();

        //sets up events for and then loads the database
        InitilizeDatabaseLoading();
        databaseRetreiver.LoadDatabase();
    }

    private void OnGUI()
    {
        DisplayDatabase();
        DisplayCurrent();
    }

    private void InitilizeDatabaseLoading()
    {
        databaseRetreiver = new HatDatabaseRetreiver();
        databaseRetreiver.OnDatabaseLoad.AddListener(SetLoadedDatabase);
    }
    private void SetLoadedDatabase(HatDatabase loadedDatabase)
    {
        _loadedList = loadedDatabase.GetDatabaseCopy();
    }

    private void DisplayCurrent()
    {
        EditorGUI.DrawRect(_currentHat, listColor1);
        EditorGUI.DrawRect(_currentHatfieldsRect, listColor2);

        //validation Text
        EditorGUI.LabelField(_vallidationRect, _vallidationMessage, _validationTextStyle);

        //Save Button
        if(ValidateCurrent())
        {
            if(_overwriteMode)
            {

                if (GUI.Button(new Rect(_addToDatabaseRect.x, _addToDatabaseRect.y, _addToDatabaseRect.width * 0.5f, _addToDatabaseRect.height), "Overwrite Hat"))
                {
                    AddCurrentToDatabase();
                    LoadEditorFromDatabase();
                }
            }
            else
            {
                if (GUI.Button(_addToDatabaseRect, "Save Hat"))
                {
                    AddCurrentToDatabase();
                    LoadEditorFromDatabase();
                }
            }

        }
        if(_overwriteMode)
        {

            if (GUI.Button(new Rect(_addToDatabaseRect.width * 0.5f, _addToDatabaseRect.y, _addToDatabaseRect.width * 0.5f, _addToDatabaseRect.height), "Erase HatData"))
            {
                EraseHatData();
                LoadEditorFromDatabase();
            }
        }

        //hatname
        EditorGUI.LabelField(_currentHatNameTextRect, "Hat Name", _labelStyle);
        _currentName = EditorGUI.TextField(_currentHatNameRect, _currentName);
        //hatmesh
        EditorGUI.LabelField(_currentHatMeshTextRect, "Hat Mesh", _labelStyle);
        _currentMesh = (Mesh)EditorGUI.ObjectField(_currentHatMeshRect, _currentMesh, typeof(Mesh), false);
        //hatmaterial
        EditorGUI.LabelField(_currentHatMaterialTextRect, "Hat Material", _labelStyle);
        _currentMat = (Material)EditorGUI.ObjectField(_currentHatMaterialRect, _currentMat, typeof(Material), false);
        //ID (Not set by user)
        EditorGUI.LabelField(_iDRect, "ID#: " + _currentID.ToString(), _idStyle);
    }  

    private void DisplayDatabase()
    {
        if(_loadedList != null)
        {
            int heightValue = 25;

            for (int i = 0; i < _loadedList.Count; i++)
            {
                DisplayHatFromDatabase(i, _loadedList[i], heightValue, _displayScrollIndexing);
            }

            if(GUI.Button(new Rect(0, _currentHat.height, 500, heightValue), "^^^"))
            {
                _displayScrollIndexing = Mathf.Clamp(_displayScrollIndexing - 1, -_loadedList.Count, 1);
            }
            if (GUI.Button(new Rect(0, 500 - heightValue, 500, heightValue), "vvv"))
            {
                _displayScrollIndexing = Mathf.Clamp(_displayScrollIndexing + 1, -_loadedList.Count - 1, 1);
            }
        }
    }

    private void DisplayHatFromDatabase(int hatindex, HatData hatdata, int itemHeight, int scrollIndex)
    {
        Color listColor;
        if(hatindex % 2 == 1)
        {
            listColor = listColor1;
        } //color alternating
        else
        {
            listColor = listColor2;
        }

        Rect itemRect = new Rect(0, _currentHat.height + (hatindex * itemHeight) + itemHeight + (itemHeight * scrollIndex), 500, itemHeight);

        if (GUI.Button(itemRect, "")) //button for loading into current.
        {
            HatDataToCurrent(hatdata);
        }

        EditorGUI.DrawRect(itemRect, listColor);

        EditorGUI.LabelField(new Rect(itemRect.x + itemRect.width * 0.05f, itemRect.y, itemRect.width * 0.15f, itemRect.height), "ID: " + hatdata.hatID.ToString(), _idStyle);
        EditorGUI.LabelField(new Rect(itemRect.x + itemRect.width * 0.25f, itemRect.y, itemRect.width * 0.2f, itemRect.height), "Name: " + hatdata.hatName, _databaseDisplayStyle);
        EditorGUI.LabelField(new Rect(itemRect.x + itemRect.width * 0.5f, itemRect.y, itemRect.width * 0.2f, itemRect.height), "Mesh: " + hatdata.hatMesh.name, _databaseDisplayStyle);
        EditorGUI.LabelField(new Rect(itemRect.x + itemRect.width * 0.75f, itemRect.y, itemRect.width * 0.2f, itemRect.height), "Mat: " + hatdata.hatMaterial.name, _databaseDisplayStyle);

    }

    private bool ValidateCurrent()
    {
        _vallidationMessage = "";

        bool nameValid = true;
        bool matMeshValid = true;

        _overwriteMode = false;


        #region "Name Validation

        bool puncWarning = false; //warning flag for punctiation
        bool numWarning = false; //warning flag for numbers

        if(_currentName != null)
        {
            if (_currentName.Length < 1)
            {
                if (_vallidationMessage != "")
                {
                    _vallidationMessage += ", ";
                } //formatting 

                _vallidationMessage += "No Name on Hat";
                nameValid = false;
            }

            for (int i = 0; i < _currentName.Length; i++)
            {

                if(char.IsPunctuation(_currentName[i]) && !puncWarning)
                {
                    if (_vallidationMessage != "")
                    {
                        _vallidationMessage += ", ";
                    } //formatting

                    puncWarning = true;
                    _vallidationMessage += "Hat Name cannot Have Punctuation";

                    nameValid = false;
                }
                if (char.IsDigit(_currentName[i]) && !numWarning)
                {
                    if(_vallidationMessage != "")
                    {
                        _vallidationMessage += ", ";
                    } //formatting

                    numWarning = true;
                    _vallidationMessage += "Hat Name cannot Have Numbers";

                    nameValid = false;
                }
            }
        }
        else
        {
            if (_vallidationMessage != "")
            {
                _vallidationMessage += ", ";
            } //formatting

            _vallidationMessage += "No Name on Hat";
            nameValid = false;
        }

        if (nameValid)
        {
            _currentName = UtilityFunctions.FormatStringFirstLetterCapitalized(_currentName); //formats name
        }

        #endregion

        #region "Mesh/Mat Validation

        if(_currentMesh == null)
        {
            if (_vallidationMessage != "")
            {
                _vallidationMessage += ", ";
            } //formatting

            _vallidationMessage += "No mesh on hat";
            matMeshValid = false;
        }
        if (_currentMat == null)
        {
            if (_vallidationMessage != "")
            {
                _vallidationMessage += ", ";
            } //formatting

            _vallidationMessage += "No material on hat";
            matMeshValid = false;
        }

        #endregion

        if (CheckHatListForRepeatNames(_loadedList))
        {
            _overwriteMode = true;
        }



        //final output
        if(nameValid && matMeshValid)
        {
            //messege style change
            if (_validationTextStyle != null)
                _validationTextStyle.normal.textColor = Color.green;

            _vallidationMessage = "Hat Valid";

            if (_overwriteMode)
                _vallidationMessage = "OVERWRITING, " + _vallidationMessage;

            if (_loadedList != null && !_overwriteMode)
            {
                _currentID = DetermineCurrentHatID(_loadedList);
            }


            return true;
        }
        else
        {
            //messege style change
            if (_validationTextStyle != null)
                _validationTextStyle.normal.textColor = Color.red;

            if (_overwriteMode)
                _vallidationMessage = "OVERWRITING, " + _vallidationMessage;

            return false;
        }
    } //validates current returns false if any of the fields aren't filled in appropiatly. 

    private void InitilizeCurrentDisplay()
    {
        #region"Styles"
        //styles initilization
        _labelStyle = new GUIStyle();
        _labelStyle.alignment = TextAnchor.MiddleCenter;
        _labelStyle.fontStyle = FontStyle.Bold;
        _labelStyle.normal.textColor = Color.white;

        _idStyle = new GUIStyle();
        _idStyle.alignment = TextAnchor.MiddleLeft;
        _idStyle.fontStyle = FontStyle.Bold;
        _idStyle.normal.textColor = Color.white;
        _idStyle.fontSize = 16;

        _validationTextStyle = new GUIStyle();
        _validationTextStyle.alignment = TextAnchor.MiddleLeft;
        _validationTextStyle.fontStyle = FontStyle.Bold;
        _validationTextStyle.normal.textColor = Color.red;
        _validationTextStyle.fontSize = 12;

        _databaseDisplayStyle = new GUIStyle();
        _databaseDisplayStyle.alignment = TextAnchor.MiddleLeft;
        _databaseDisplayStyle.fontStyle = FontStyle.Bold;
        _databaseDisplayStyle.normal.textColor = Color.white;
        _databaseDisplayStyle.fontSize = 10;
        _databaseDisplayStyle.clipping = TextClipping.Clip;
        #endregion

        _currentHat = new Rect(0, 0, 500, 100);

        _vallidationRect = new Rect(_currentHat.width * 0.05f, 0, _currentHat.width * 0.9f, _currentHat.height / 4);

        _addToDatabaseRect = new Rect(_currentHat.x, _currentHat.height * 0.75f, _currentHat.width, _currentHat.height * 0.25f);

        //hatrect initilization
        _currentHatfieldsRect = new Rect(0, _currentHat.height / 4, _currentHat.width, _currentHat.height / 2);
        //name rect
        _currentHatNameRect = new Rect(_currentHatfieldsRect.width / 4 - 40, _currentHatfieldsRect.y + _currentHatfieldsRect.height / 2, _currentHatfieldsRect.width / 4, _currentHatfieldsRect.height / 2);
        _currentHatNameTextRect = LabelVerticalOffset(_currentHatNameRect); // new Rect(_currentHatNameRect.x, _currentHatNameRect.y - _currentHatRect.height / 2, _currentHatNameRect.width, _currentHatNameRect.height);
        //mesh rect, can utilize y values set up from name rects
        _currentHatMeshRect = new Rect(_currentHatfieldsRect.width - _currentHatfieldsRect.width / 2 - 30, _currentHatNameRect.y, _currentHatNameRect.width, _currentHatNameRect.height);
        _currentHatMeshTextRect = LabelVerticalOffset(_currentHatMeshRect);
        //mat rect
        _currentHatMaterialRect = new Rect(_currentHatfieldsRect.width - _currentHatfieldsRect.width / 4 - 20, _currentHatNameRect.y, _currentHatNameRect.width, _currentHatNameRect.height);
        _currentHatMaterialTextRect = LabelVerticalOffset(_currentHatMaterialRect);
        //ID rect
        _iDRect = new Rect(10, _currentHatNameRect.y, _currentHatNameRect.width - 50, _currentHatNameRect.height);

        _currentHatfieldsRect.height += 5; //gives some padding at the bottom after caculations for other elements.
    }

    #region"Display Helper Functions"

    private Rect LabelVerticalOffset(Rect fieldRect)
    {
        return new Rect(fieldRect.x, fieldRect.y - _currentHatfieldsRect.height / 2, fieldRect.width, fieldRect.height);
    }

    #endregion


    #region"Conversions
    private HatData CurrentToHatData()
    {
        return new HatData(_currentID, _currentName, _currentMesh, _currentMat);
    }
    private void HatDataToCurrent(HatData data)
    {
        _currentID = data.hatID;
        _currentName = data.hatName;
        _currentMesh = data.hatMesh;
        _currentMat = data.hatMaterial;
    }
    #endregion


    #region "Save/Load/Erase Functions"

    //also overwrites 
    private void AddCurrentToDatabase()
    {
        if(databaseWriter == null)
        {
            databaseWriter = new HatDatabaseWriter();
        }
        databaseWriter.AddToDataBase(CurrentToHatData());
    }

    private void LoadEditorFromDatabase()
    {
        if(databaseRetreiver == null)
        {
            InitilizeDatabaseLoading();
        }
        databaseRetreiver.LoadDatabase();
        System.Threading.Thread.Sleep(1000);
    }

    //works on ID
    private void EraseHatData()
    {
        if (databaseWriter == null)
        {
            databaseWriter = new HatDatabaseWriter();
        }
        databaseWriter.EraseHat(_currentID);
    }
    
    #endregion

    private bool CheckHatListForRepeatNames(List<HatData> hatList)
    {
        if (hatList != null)
        {
            for (int i = 0; i < hatList.Count; i++)
            {
                if (hatList[i].hatName == _currentName)
                {
                    _currentID = hatList[i].hatID;
                    return true;
                }
            }
        }

        DetermineCurrentHatID(_loadedList);
        return false;
    }

    //ran in save and load.
    private int DetermineCurrentHatID(List<HatData> hatList)
    {
        int id = 1;

        if (hatList != null)
        {
            for (int i = 0; i < hatList.Count; i++)
            {
                if (hatList[i].hatID >= id)
                {
                    id = hatList[i].hatID + 1;
                }
            }
        }

        return id;
    }

    private void OnDestroy()
    {
        _windowOpen = false;
    }


}
#endif


//single chunck of hat data, filled out when created using it's constructor.
[System.Serializable]
public class HatData
{
    public int hatID;
    public string hatName;
    public Mesh hatMesh;
    public Material hatMaterial;

    public HatData(int ID, string name, Mesh mesh, Material material)
    {
        hatID = ID;
        hatName = name;
        hatMesh = mesh;
        hatMaterial = material;
    }

    public HatData GetCopy()
    {
        return new HatData(hatID, hatName, hatMesh, hatMaterial);
    }
}

