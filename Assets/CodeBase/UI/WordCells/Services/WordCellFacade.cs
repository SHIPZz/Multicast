using System;
using CodeBase.Common.Services.Persistent;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.Gameplay.Constants;
using Zenject;

namespace CodeBase.UI.WordCells.Services
{
    public class WordCellFacade : IWordSlotFacade, IInitializable, IDisposable
    {
        private readonly IPersistentService _persistentService;
        private readonly IWordCellRepository _wordCellRepository;
        private readonly IWordCellChecker _wordCellChecker;

        public WordCellFacade(IPersistentService persistentService, IWordCellRepository wordCellRepository, IWordCellChecker wordCellChecker)
        {
            _persistentService = persistentService;
            _wordCellRepository = wordCellRepository;
            _wordCellChecker = wordCellChecker;
        }

        public void Initialize()
        {
            _persistentService.RegisterProgressWatcher(this);
        }

        public void Dispose()
        {
            _persistentService.UnregisterProgressWatcher(this);
        }

        public void Cleanup()
        {
            _wordCellRepository.ClearAllCells();
            _wordCellChecker.Cleanup();
        }

        public void Save(ProgressData progressData)
        {
            if(_wordCellRepository.CreatedSlots.IsNullOrEmpty())
                return;
            
            var playerData = progressData.PlayerData;
            playerData.WordsToFind.Clear();
            playerData.WordsToFind.AddRange(_wordCellChecker.TargetWordsToFind);

            playerData.WordSlotsGrid = new string[GameplayConstants.MaxWordCount, GameplayConstants.MaxClustersInColumn];

            for (int row = 0; row < playerData.WordSlotsGrid.GetLength(0); row++)
            {
                for (int col = 0; col < playerData.WordSlotsGrid.GetLength(1); col++)
                {
                    WordCellView cell = _wordCellRepository.GetWordSlotByRowAndColumn(row, col);
                    playerData.WordSlotsGrid[row, col] = cell.IsOccupied ? cell.Letter.ToString() : string.Empty;
                }
            }
        }

        public void Load(ProgressData progressData)
        {
            Cleanup();
            
            _wordCellChecker.Init(progressData.PlayerData.WordsToFind);

            if (progressData.PlayerData.WordSlotsGrid != null)
                _wordCellRepository.RestoreState(progressData.PlayerData.WordSlotsGrid);
        }
    }
}