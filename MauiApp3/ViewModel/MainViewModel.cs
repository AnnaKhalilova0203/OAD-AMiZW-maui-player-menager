using MauiApp3.Commands;
using MauiApp3.Model;
using MauiApp3.Queries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiApp3.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly GetPlayerByIdQueryHandler _getPlayerByIdQueryHandler;
        private readonly GetPlayersQueryHandler _getPlayersQueryHandler;
        private readonly AddPlayerCommandHandler _addPlayerCommandHandler;
        private readonly DeletePlayerCommandHandler _deletePlayerCommandHandler;
        private readonly EditPlayerCommandHandler _editPlayerCommandHandler;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Player> Players { get; } = new();

        private string _playerName = "";
        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }

        private string _playerLevel = "";
        public string PlayerLevel
        {
            get => _playerLevel;
            set
            {
                _playerLevel = value;
                OnPropertyChanged();
            }
        }

        private string _playerId = "";
        public string PlayerId
        {
            get => _playerId;
            set
            {
                _playerId = value;
                OnPropertyChanged();
            }
        }

        private string _deletePlayerId = "";
        public string DeletePlayerId
        {
            get => _deletePlayerId;
            set
            {
                _deletePlayerId = value;
                OnPropertyChanged();
            }
        }

        private string _editPlayerId = "";
        public string EditPlayerId
        {
            get => _editPlayerId;
            set
            {
                _editPlayerId = value;
                OnPropertyChanged();
            }
        }

        private string _editPlayerName = "";
        public string EditPlayerName
        {
            get => _editPlayerName;
            set
            {
                _editPlayerName = value;
                OnPropertyChanged();
            }
        }

        private string _editPlayerLevel = "";
        public string EditPlayerLevel
        {
            get => _editPlayerLevel;
            set
            {
                _editPlayerLevel = value;
                OnPropertyChanged();
            }
        }

        private string _searchResult = "";
        public string SearchResult
        {
            get => _searchResult;
            set
            {
                _searchResult = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddPlayerCommandButton { get; }
        public ICommand FindPlayerCommandButton { get; }
        public ICommand DeletePlayerCommandButton { get; }
        public ICommand EditPlayerCommandButton { get; }

        public MainViewModel(
            GetPlayerByIdQueryHandler getPlayerByIdQueryHandler,
            GetPlayersQueryHandler getPlayersQueryHandler,
            AddPlayerCommandHandler addPlayerCommandHandler,
            DeletePlayerCommandHandler deletePlayerCommandHandler,
            EditPlayerCommandHandler editPlayerCommandHandler)
        {
            _getPlayerByIdQueryHandler = getPlayerByIdQueryHandler;
            _getPlayersQueryHandler = getPlayersQueryHandler;
            _addPlayerCommandHandler = addPlayerCommandHandler;
            _deletePlayerCommandHandler = deletePlayerCommandHandler;
            _editPlayerCommandHandler = editPlayerCommandHandler;

            AddPlayerCommandButton = new Command(AddPlayer);
            FindPlayerCommandButton = new Command(FindPlayer);
            DeletePlayerCommandButton = new Command(DeletePlayer);
            EditPlayerCommandButton = new Command(EditPlayer);

            LoadPlayers();
        }

        private void AddPlayer()
        {
            if (string.IsNullOrWhiteSpace(PlayerName))
            {
                SearchResult = "Podaj nazwę gracza.";
                return;
            }

            if (!int.TryParse(PlayerLevel, out int playerLevel)
                || playerLevel < 0)
            {
                SearchResult = "Podaj poprawny, nieujemny poziom.";
                return;
            }

            AddPlayerCommand command =
                new AddPlayerCommand(PlayerName.Trim(), playerLevel);

            _addPlayerCommandHandler.Handle(command);

            LoadPlayers();

            PlayerName = "";
            PlayerLevel = "";
            SearchResult = "Dodano gracza.";
        }

        private void FindPlayer()
        {
            if (!int.TryParse(PlayerId, out int playerId)
                || playerId <= 0)
            {
                SearchResult = "Podaj poprawne ID gracza.";
                return;
            }

            GetPlayerByIdQuery query =
                new GetPlayerByIdQuery(playerId);

            Player? player = _getPlayerByIdQueryHandler.Handle(query);

            if (player == null)
            {
                SearchResult = "Nie znaleziono gracza.";
                return;
            }

            SearchResult =
                $"{player.Name} - poziom {player.Level}";
        }

        private void DeletePlayer()
        {
            if (!int.TryParse(DeletePlayerId, out int playerId)
                || playerId <= 0)
            {
                SearchResult = "Podaj poprawne ID gracza do usunięcia.";
                return;
            }

            bool deleted = _deletePlayerCommandHandler.Handle(
                new DeletePlayerCommand(playerId));

            if (!deleted)
            {
                SearchResult = "Nie znaleziono gracza o podanym ID.";
                return;
            }

            LoadPlayers();

            DeletePlayerId = "";
            SearchResult = "Usunięto gracza.";
        }

        private void EditPlayer()
        {
            if (!int.TryParse(EditPlayerId, out int playerId)
                || playerId <= 0)
            {
                SearchResult = "Podaj poprawne ID gracza do edycji.";
                return;
            }

            if (string.IsNullOrWhiteSpace(EditPlayerName))
            {
                SearchResult = "Podaj nową nazwę gracza.";
                return;
            }

            if (!int.TryParse(EditPlayerLevel, out int playerLevel)
                || playerLevel < 0)
            {
                SearchResult = "Podaj poprawny, nieujemny poziom.";
                return;
            }

            bool edited = _editPlayerCommandHandler.Handle(
                new EditPlayerCommand(
                    playerId,
                    EditPlayerName.Trim(),
                    playerLevel));

            if (!edited)
            {
                SearchResult = "Nie znaleziono gracza o podanym ID.";
                return;
            }

            LoadPlayers();

            EditPlayerId = "";
            EditPlayerName = "";
            EditPlayerLevel = "";

            SearchResult = "Zaktualizowano gracza.";
        }

        private void LoadPlayers()
        {
            GetPlayersQuery query = new GetPlayersQuery();
            List<Player> players = _getPlayersQueryHandler.Handle(query);

            Players.Clear();

            foreach (Player player in players)
            {
                Players.Add(player);
            }
        }

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}