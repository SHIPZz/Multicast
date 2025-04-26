using System;
using CodeBase.Common.Services.Persistent;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.Gameplay.Constants;
using Zenject;

namespace CodeBase.UI.WordSlots.Services
{
    public class WordSlotFacade : IWordSlotFacade, IInitializable, IDisposable
    {
        private readonly IPersistentService _persistentService;
        private readonly IWordSlotRepository _wordSlotRepository;
        private readonly IWordSlotChecker _wordSlotChecker;

        public WordSlotFacade(IPersistentService persistentService, IWordSlotRepository wordSlotRepository, IWordSlotChecker wordSlotChecker)
        {
            _persistentService = persistentService;
            _wordSlotRepository = wordSlotRepository;
            _wordSlotChecker = wordSlotChecker;
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
            _wordSlotRepository.ClearAllCells();
            _wordSlotChecker.Cleanup();
        }

        public void Save(ProgressData progressData)
        {
            if(_wordSlotRepository.CreatedSlots.IsNullOrEmpty())
                return;
            
            var playerData = progressData.PlayerData;
            playerData.WordsToFind.Clear();
            playerData.WordsToFind.AddRange(_wordSlotChecker.TargetWordsToFind);

            playerData.WordSlotsGrid = new string[GameplayConstants.MaxWordCount, GameplayConstants.MaxClustersInColumn];

            for (int row = 0; row < playerData.WordSlotsGrid.GetLength(0); row++)
            {
                for (int col = 0; col < playerData.WordSlotsGrid.GetLength(1); col++)
                {
                    WordSlot cell = _wordSlotRepository.GetWordSlotByRowAndColumn(row, col);
                    playerData.WordSlotsGrid[row, col] = cell.IsOccupied ? cell.Letter.ToString() : string.Empty;
                }
            }
        }

        public void Load(ProgressData progressData)
        {
            Cleanup();
            
            _wordSlotChecker.Init(progressData.PlayerData.WordsToFind);

            if (progressData.PlayerData.WordSlotsGrid != null)
                _wordSlotRepository.RestoreState(progressData.PlayerData.WordSlotsGrid);
        }
    }
}