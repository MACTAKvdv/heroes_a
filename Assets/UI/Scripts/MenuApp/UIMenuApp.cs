using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using System.Linq;
using UnityEngine.Localization;

public class UIMenuApp : UILocaleBase
{
    [SerializeField] private UIDocument _menuAppDoc;
    public UIDocument MenuApp => _menuAppDoc;

    // [SerializeField] private MapManager mapGenerator;

    // public UnityAction OnQuit;
    private VisualElement buttonsSection;

    [SerializeField] private UIDocument _menuNewGameDoc;
    [SerializeField] private VisualTreeAsset _templateButton;
    [SerializeField] private VisualTreeAsset _templateColAdvance;
    [SerializeField] private VisualTreeAsset _templateColAdvanceChooser;
    [SerializeField] private VisualTreeAsset _templateButtonWithImg;
    [SerializeField] private VisualTreeAsset _templateColumn;
    public UIDocument MenuNewGame => _menuNewGameDoc;

    // [SerializeField] public LevelManager _levelManager;
    // public LevelManager LevelManager => _levelManager;

    private VisualElement _box;
    private VisualElement _boxOptions;
    private Button _btnNewGame;

    private string _currentMode;

    private string nameNameOption = "NameOption";
    private string nameBoxOption = "BoxOption";

    private string nameTabActive = "tab_active";
    private string nameTabGeneral = "General";
    private Button _btnTabGeneral;
    private string nameTabContentGeneral = "TabContentGeneral";
    private VisualElement _tabContentGeneral;

    private string nameTabAdvance = "Advance";
    private Button _btnTabAdvance;
    private string nameBoxContentSetting = "BoxSetting";
    private VisualElement _boxContentSetting;
    private VisualElement _boxContentGeneral;
    private VisualElement _boxContentAdvanced;
    private VisualElement _buttonRandomSetting;
    private VisualElement _buttonAdvancedSetting;
    private VisualElement _complexityBox;
    private List<ScriptableGameMode> _listGameMode;
    private ScriptableGameSetting _settings;

    private List<ScriptableEntityHero> _listChoosedHero = new List<ScriptableEntityHero>();
    private List<ScriptableEntityHero> _listHeroes = new List<ScriptableEntityHero>();

    private GameObject _environment;

    public void Init(GameObject environment)
    {
        LevelManager.Instance.Init();

        _environment = environment;

        _listGameMode = ResourceSystem.Instance.GetGameMode();
        _listHeroes = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero);

        _settings = ResourceSystem.Instance
            .GetAllAssetsByLabel<ScriptableGameSetting>(Constants.Labels.LABEL_GAMESETTING)[0];


        buttonsSection = MenuApp.rootVisualElement.Q<VisualElement>("ButtonsSection");

        var newGameButton = MenuApp.rootVisualElement.Q<Button>("newgame");
        newGameButton.clickable.clicked += () =>
        {
            // GameManager.Instance.ChangeState(GameState.NewGame);
            ShowNewGameWindow();
        };

        var loadGameButton = MenuApp.rootVisualElement.Q<Button>("loadgame");
        loadGameButton.clickable.clicked += async () =>
        {
            GameManager.Instance.ChangeState(GameState.LoadGame);
            HideNewGameWindow();
            await GameManager.Instance.AssetProvider.UnloadAsset(_environment);
        };

        var btnQuit = MenuApp.rootVisualElement.Q<Button>("ButtonQuit");
        btnQuit.clickable.clicked += () =>
        {
            Application.Quit();
        };

        // new game window
        _box = _menuNewGameDoc.rootVisualElement.Q<VisualElement>("NewGameBox");
        _complexityBox = _box.Q<VisualElement>("Complexity");
        _boxContentGeneral = _box.Q<VisualElement>("BoxGeneral");
        _boxContentAdvanced = _box.Q<VisualElement>("BoxAdvanced");
        _buttonAdvancedSetting = _box.Q<VisualElement>("AdvancedOptions");
        _buttonAdvancedSetting.Q<Button>().clickable.clicked += () =>
        {
            ShowHideAdvancedOptions();
        };
        _boxContentSetting = _box.Q<VisualElement>(nameBoxContentSetting);
        _buttonRandomSetting = _box.Q<VisualElement>("RandomOptions");
        _buttonRandomSetting.Q<Button>().clickable.clicked += () =>
        {
            ShowHideRandomOptions();
        };

        ShowHideRandomOptions();
        ShowHideAdvancedOptions();
        // _tabContentAdvance = _box.Q<VisualElement>(nameTabContentAdvance);
        // _btnTabAdvance = _box.Q<Button>(nameTabAdvance);
        // _btnTabAdvance.clickable.clicked += () =>
        // {
        //     _tabContentGeneral.style.display = DisplayStyle.None;
        //     _btnTabGeneral.RemoveFromClassList(nameTabActive);
        //     _tabContentAdvance.style.display = DisplayStyle.Flex;
        //     _btnTabAdvance.AddToClassList(nameTabActive);

        // };

        // _tabContentGeneral = _box.Q<VisualElement>(nameTabContentGeneral);
        // _btnTabGeneral = _box.Q<Button>(nameTabGeneral);
        // _btnTabGeneral.clickable.clicked += () =>
        // {
        //     _tabContentGeneral.style.display = DisplayStyle.Flex;
        //     _btnTabGeneral.AddToClassList(nameTabActive);
        //     _tabContentAdvance.style.display = DisplayStyle.None;
        //     _btnTabAdvance.RemoveFromClassList(nameTabActive);
        // };

        _btnNewGame = _box.Q<VisualElement>("ButtonNewGame").Q<Button>("Btn");
        _btnNewGame.clickable.clicked += async () =>
        {
            GameManager.Instance.ChangeState(GameState.NewGame);
            HideNewGameWindow();
            await GameManager.Instance.AssetProvider.UnloadAsset(_environment);
        };

        var btnClose = _menuNewGameDoc.rootVisualElement.Q<VisualElement>("ButtonClose").Q<Button>("Btn");
        btnClose.clickable.clicked += () =>
        {
            HideNewGameWindow();
        };
        // MenuApp.rootVisualElement.Add(_menuNewGameDoc.rootVisualElement);

        HideNewGameWindow();
        RefreshOptions();

        base.Localize(MenuApp.rootVisualElement);

    }

    private void ShowHideAdvancedOptions()
    {
        if (_boxContentAdvanced.style.display == DisplayStyle.None)
        {
            DrawAdvancedOptions();
            _boxContentAdvanced.style.display = DisplayStyle.Flex;
            _boxContentSetting.style.display = DisplayStyle.None;
        }
        else
        {
            _boxContentAdvanced.style.display = DisplayStyle.None;
        }
    }
    private void ShowHideRandomOptions()
    {
        if (_boxContentSetting.style.display == DisplayStyle.None)
        {
            _boxContentSetting.style.display = DisplayStyle.Flex;
            _boxContentAdvanced.style.display = DisplayStyle.None;
        }
        else
        {
            _boxContentSetting.style.display = DisplayStyle.None;
        }
    }

    private void DrawAdvancedOptions()
    {
        var _boxContentAdvancedList = _box.Q<ScrollView>("BoxAdvancedOptions");
        _boxContentAdvancedList.Clear();

        var Level = LevelManager.Instance.Level;
        List<ScriptableEntityTown> listTowns = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityTown>(TypeEntity.Town);

        foreach (var player in Level.Settings.Players)
        {

            var newCol = _templateColAdvance.Instantiate();
            var listChoosers = newCol.Q<VisualElement>("ListChoosers");
            listChoosers.Clear();
            // newCol.Q<Label>(nameNameOption).text = player.DataPlayer.id.ToString();
            Color color = player.DataPlayer.color;
            color.a = 0.8f;
            newCol.Q<VisualElement>("Overlay").style.backgroundColor
                = color;

            // Create chooser town.
            var activeTown = listTowns.IndexOf(player.StartSetting.town);
            var chooserTown = _templateColAdvanceChooser.Instantiate();
            chooserTown.AddToClassList("w-33");
            chooserTown.AddToClassList("px-1");
            if (player.StartSetting.town != null)
            {
                SetImg(player.StartSetting.town, chooserTown);
            }
            else
            {
                SetImg(null, chooserTown);
                _listChoosedHero.Remove(player.StartSetting.hero);
                player.StartSetting.hero = null;
            }
            chooserTown.Q<Button>("arrowleft").clickable.clicked += () =>
            {
                if (activeTown >= 0)
                {
                    activeTown--;
                }
                else if (activeTown == -1)
                {
                    activeTown = listTowns.Count - 1;
                }
                if (activeTown >= 0 && activeTown < listTowns.Count)
                {
                    player.StartSetting.town = listTowns[activeTown];
                    _listChoosedHero.Remove(player.StartSetting.hero);
                    player.StartSetting.hero = null;
                }
                else
                {
                    player.StartSetting.town = null;
                }

                DrawAdvancedOptions();
            };
            chooserTown.Q<Button>("arrowright").clickable.clicked += () =>
            {
                if (activeTown <= listTowns.Count - 1)
                {
                    activeTown++;
                }
                else if (activeTown == listTowns.Count)
                {
                    activeTown = 0;
                }
                if (activeTown >= 0 && activeTown < listTowns.Count)
                {
                    player.StartSetting.town = listTowns[activeTown];
                    _listChoosedHero.Remove(player.StartSetting.hero);
                    player.StartSetting.hero = null;
                }
                else
                {
                    player.StartSetting.town = null;
                }

                DrawAdvancedOptions();
            };
            listChoosers.Add(chooserTown);

            // Create chooser hero.
            List<ScriptableEntityHero> listHeroesForTown
                = player.StartSetting.town == null
                    ? new List<ScriptableEntityHero>()
                    : _listHeroes
                        .Where(t =>
                        t.TypeFaction == player.StartSetting.town.TypeFaction
                        && (!_listChoosedHero.Contains(t) || t == player.StartSetting.hero)
                        )
                        .ToList();
            var indexActiveHero = listHeroesForTown.IndexOf(player.StartSetting.hero);

            var chooserHero = _templateColAdvanceChooser.Instantiate();
            chooserHero.AddToClassList("w-33");
            chooserHero.AddToClassList("px-1");
            if (player.StartSetting.hero != null)
            {
                SetImg(player.StartSetting.hero, chooserHero);
            }
            else
            {
                SetImg(null, chooserHero);
            }
            var btnPrevHero = chooserHero.Q<Button>("arrowleft");
            var btnNextHero = chooserHero.Q<Button>("arrowright");

            if (player.StartSetting.town != null)
            {
                btnPrevHero.clickable.clicked += () =>
                {
                    if (indexActiveHero >= 0)
                    {
                        indexActiveHero--;
                    }
                    else if (indexActiveHero == -1)
                    {
                        indexActiveHero = listHeroesForTown.Count - 1;
                    }
                    if (indexActiveHero >= 0 && indexActiveHero < listHeroesForTown.Count)
                    {
                        _listChoosedHero.Remove(player.StartSetting.hero);
                        player.StartSetting.hero = listHeroesForTown[indexActiveHero];
                        _listChoosedHero.Add(player.StartSetting.hero);
                    }
                    else
                    {
                        _listChoosedHero.Remove(player.StartSetting.hero);
                        player.StartSetting.hero = null;
                    }

                    DrawAdvancedOptions();
                };
                btnNextHero.clickable.clicked += () =>
                {
                    if (indexActiveHero <= listHeroesForTown.Count - 1)
                    {
                        indexActiveHero++;
                    }
                    else if (indexActiveHero == listHeroesForTown.Count)
                    {
                        indexActiveHero = 0;
                    }
                    if (indexActiveHero >= 0 && indexActiveHero < listHeroesForTown.Count)
                    {
                        _listChoosedHero.Remove(player.StartSetting.hero);
                        player.StartSetting.hero = listHeroesForTown[indexActiveHero];
                        _listChoosedHero.Add(player.StartSetting.hero);
                    }
                    else
                    {
                        _listChoosedHero.Remove(player.StartSetting.hero);
                        player.StartSetting.hero = null;
                    }

                    DrawAdvancedOptions();
                };
            }
            else
            {
                btnPrevHero.SetEnabled(false);
                btnNextHero.SetEnabled(false);
            }
            listChoosers.Add(chooserHero);

            // Create chooser bonus.
            var chooserBonus = _templateColAdvanceChooser.Instantiate();
            chooserBonus.AddToClassList("w-33");
            chooserBonus.AddToClassList("px-1");

            SetBonus(player.StartSetting.bonus, chooserBonus);
            var indexActiveBonus = _settings.StartBonuses
                .FindIndex(t => t.bonus == player.StartSetting.bonus);
            chooserBonus.Q<Button>("arrowleft").clickable.clicked += () =>
            {
                if (indexActiveBonus >= 0)
                {
                    indexActiveBonus--;
                }
                else if (indexActiveBonus == -1)
                {
                    indexActiveBonus = _settings.StartBonuses.Count - 1;
                }
                if (indexActiveBonus >= 0 && indexActiveBonus < _settings.StartBonuses.Count)
                {
                    player.StartSetting.bonus = _settings.StartBonuses[indexActiveBonus].bonus;
                }
                else
                {
                    player.StartSetting.bonus = TypeStartBonus.None;
                }

                DrawAdvancedOptions();
            };
            chooserBonus.Q<Button>("arrowright").clickable.clicked += () =>
            {
                if (indexActiveBonus <= _settings.StartBonuses.Count - 1)
                {
                    indexActiveBonus++;
                }
                else if (indexActiveBonus == _settings.StartBonuses.Count)
                {
                    indexActiveBonus = 0;
                }
                if (indexActiveBonus >= 0 && indexActiveBonus < _settings.StartBonuses.Count)
                {
                    player.StartSetting.bonus = _settings.StartBonuses[indexActiveBonus].bonus;
                }
                else
                {
                    player.StartSetting.bonus = TypeStartBonus.None;
                }

                DrawAdvancedOptions();
            };
            listChoosers.Add(chooserBonus);

            _boxContentAdvancedList.Add(newCol);
        }

        base.Localize(_box);
    }

    private void SetImg(ScriptableEntity entity, VisualElement chooser)
    {
        if (entity != null)
        {
            var img = chooser.Q<VisualElement>("img");
            img.style.backgroundImage
                = new StyleBackground(entity.MenuSprite);
            chooser.Q<VisualElement>("img").style.display = DisplayStyle.Flex;
            var text = chooser.Q<Label>("text");
            text.text = entity.title.GetLocalizedString();
        }
        else
        {
            chooser.Q<VisualElement>("img").style.display = DisplayStyle.None;
            var text = chooser.Q<Label>("text");
            text.text = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "random").GetLocalizedString();
        }
    }

    private void SetBonus(TypeStartBonus typeBonus, VisualElement chooser)
    {
        var bonuses = _settings.StartBonuses.Where(t => t.bonus == typeBonus);
        if (bonuses.Count() > 0)
        {
            var bonus = bonuses.First();
            var img = chooser.Q<VisualElement>("img");
            img.style.backgroundImage
                = new StyleBackground(bonus.sprite);
            chooser.Q<VisualElement>("img").style.display = DisplayStyle.Flex;
            var text = chooser.Q<Label>("text");
            text.text = bonus.title.GetLocalizedString();
        }
        else
        {
            chooser.Q<VisualElement>("img").style.display = DisplayStyle.None;
            var text = chooser.Q<Label>("text");
            text.text = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "random").GetLocalizedString();
        }
    }

    private void RefreshOptions()
    {
        var Level = LevelManager.Instance.Level;

        _boxOptions = _box.Q<ScrollView>("BoxOptions");
        _boxOptions.Clear();

        CreateListGameMode();
        AddOptionCountPlay();
        AddOptionCountCommand();
        AddOptionCountBot();
        AddCompexity();

        if (
            (Level.Settings.countPlayer < 2 && Level.Settings.countBot == 0)
            ||
            (Level.GameModeData.title == "")
            ||
            (Level.Settings.compexity < 99)
            )
        {
            _btnNewGame.SetEnabled(false);
        }
        else
        {
            _btnNewGame.SetEnabled(true);
        }
        base.Localize(_box);
    }

    //private void AddOptionSizeArea()
    //{
    //    var newCol = _templateColumn.Instantiate();
    //    newCol.Q<Label>(nameNameOption).text = "#sizearea";

    //    var NewColBoxBtn = newCol.Q<VisualElement>(nameBoxOption);
    //    NewColBoxBtn.Clear();

    //    var newSlider = new Slider();
    //    newSlider.value = LevelManager.Instance.gameModeData.koofSizeArea;
    //    newSlider.highValue = 1f;
    //    newSlider.lowValue = 0f;
    //    newSlider.style.flexGrow = 1;


    //    newSlider.RegisterValueChangedCallback(x =>
    //    {
    //        LevelManager.Instance.gameModeData.koofSizeArea = x.newValue;
    //    });
    //    NewColBoxBtn.Add(newSlider);

    //    _boxOptions.Add(newCol);
    //}
    public void AddCompexity()
    {
        var Level = LevelManager.Instance.Level;
        var _complexityList = _complexityBox.Q<VisualElement>("ComplexityList");
        var _complexityValue = _complexityBox.Q<Label>("ComplexityValue");
        _complexityList.Clear();

        foreach (var item in _settings.Complexities)
        {
            var btnTemplate = _templateButtonWithImg.Instantiate();
            var btn = btnTemplate.Q<Button>();
            btn.Q<VisualElement>("img").style.backgroundImage
                = new StyleBackground(item.sprite);
            btn.AddToClassList("w-125");
            btn.style.height = new StyleLength(new Length(70, LengthUnit.Pixel));

            if (Level.Settings.compexity == item.value)
            {
                btn.AddToClassList("button_checked");
                btn.RemoveFromClassList("button_bg");
                btn.RemoveFromClassList("button_bordered");
            }
            else
            {
                btn.clickable.clicked += () =>
                {
                    Level.Settings.compexity = item.value;

                    RefreshOptions();
                };
            }

            _complexityList.Add(btn);
        }

        _complexityValue.text = Level.Settings.compexity + "%";

    }

    public void CreateListGameMode()
    {
        var Level = LevelManager.Instance.Level;
        var boxActiveMode = _box.Q<Label>("ActiveSizeMap");
        var newCol = _templateColumn.Instantiate();
        newCol.Q<Label>(nameNameOption).text = "#sizemap";

        var NewColBoxBtn = newCol.Q<VisualElement>(nameBoxOption);
        NewColBoxBtn.Clear();

        foreach (ScriptableGameMode mode in _listGameMode)
        {
            ScriptableGameMode currentMode = mode;
            var newButtonBox = _templateButton.Instantiate();
            var btn = newButtonBox.Q<Button>("Btn");
            btn.text = currentMode.GameModeData.title;

            if (currentMode.GameModeData.title == Level.GameModeData.title)
            {
                btn.AddToClassList("button_checked");
                btn.RemoveFromClassList("button_bg");
                btn.RemoveFromClassList("button_bordered");

                boxActiveMode.text = currentMode.GameModeData.title;
            }
            else
            {
                btn.clickable.clicked += () =>
                {
                    Level.GameModeData = currentMode.GameModeData;
                    RefreshOptions();
                };
            }
            NewColBoxBtn.Add(newButtonBox);
        }

        _boxOptions.Add(newCol);
    }

    // private void OnDestroy()
    // {
    //     foreach (var item in _listGameMode)
    //     {
    //         //Addressables.Release(item);
    //         Addressables.ReleaseInstance(this.gameObject);
    //     }
    // }

    private void AddOptionCountCommand()
    {
        var Level = LevelManager.Instance.Level;
        var newCol = _templateColumn.Instantiate();
        newCol.Q<Label>(nameNameOption).text = "#countcommand";

        var NewColBoxBtn = newCol.Q<VisualElement>(nameBoxOption);
        NewColBoxBtn.Clear();

        for (int i = 0; i <= LevelManager.Instance.GameSettings.maxPlayer - 1; i++)
        {
            var newButtonBox = _templateButton.Instantiate();
            var newBtn = newButtonBox.Q<Button>("Btn");
            newBtn.text = i.ToString();
            newBtn.name = i.ToString();
            var j = i;

            if (Level.Settings.countCommand == i)
            {
                newBtn.AddToClassList("button_checked");
                newBtn.RemoveFromClassList("button_bg");
                newBtn.RemoveFromClassList("button_bordered");
            }
            else
            {
                newBtn.clickable.clicked += () =>
                {
                    Debug.Log($"Click {j} button!");
                    Level.Settings.countCommand = j;
                    RefreshOptions();
                };
            }
            NewColBoxBtn.Add(newButtonBox);
        }

        _boxOptions.Add(newCol);
    }

    private void AddOptionCountPlay()
    {
        var Level = LevelManager.Instance.Level;
        var newCol = _templateColumn.Instantiate();
        newCol.Q<Label>(nameNameOption).text = "#countplayer";

        var NewColBoxBtn = newCol.Q<VisualElement>(nameBoxOption);
        NewColBoxBtn.Clear();

        for (int i = 1; i <= LevelManager.Instance.GameSettings.maxPlayer; i++)
        {
            var newButtonBox = _templateButton.Instantiate();
            var newBtn = newButtonBox.Q<Button>("Btn");
            newBtn.text = i.ToString();
            newBtn.name = i.ToString();
            var j = i;

            if (Level.Settings.countPlayer == i)
            {
                newBtn.AddToClassList("button_checked");
                newBtn.RemoveFromClassList("button_bg");
                newBtn.RemoveFromClassList("button_bordered");
            }
            else
            {
                newBtn.clickable.clicked += () =>
                {
                    Debug.Log($"Click {j} button!");
                    Level.Settings.countPlayer = j;
                    LevelManager.Instance.CreateListPlayer();
                    RefreshOptions();
                };
            }
            NewColBoxBtn.Add(newButtonBox);
        }

        _boxOptions.Add(newCol);
    }
    private void AddOptionCountBot()
    {
        var Level = LevelManager.Instance.Level;
        var newCol = _templateColumn.Instantiate();
        newCol.Q<Label>(nameNameOption).text = "#countbot";

        var NewColBoxBtn = newCol.Q<VisualElement>(nameBoxOption);
        NewColBoxBtn.Clear();

        int maxCountBot = LevelManager.Instance.GameSettings.maxPlayer - Level.Settings.countPlayer;

        if (Level.Settings.countBot > maxCountBot)
        {
            Level.Settings.countBot = maxCountBot;
        }

        for (int i = 0; i <= maxCountBot; i++)
        {
            var newButtonBox = _templateButton.Instantiate();
            var newBtn = newButtonBox.Q<Button>("Btn");
            newBtn.text = i.ToString();
            newBtn.name = i.ToString();
            var j = i;

            if (Level.Settings.countBot == i)
            {
                newBtn.AddToClassList("button_checked");
                newBtn.RemoveFromClassList("button_bg");
                newBtn.RemoveFromClassList("button_bordered");
            }
            else
            {
                newBtn.clickable.clicked += () =>
                {
                    Level.Settings.countBot = j;
                    RefreshOptions();
                };
            }
            NewColBoxBtn.Add(newButtonBox);
        }


        _boxOptions.Add(newCol);
    }

    private void ShowNewGameWindow()
    {
        _menuNewGameDoc.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    public void HideNewGameWindow()
    {
        _menuNewGameDoc.rootVisualElement.style.display = DisplayStyle.None;
    }
}

