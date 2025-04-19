using System;
using System.Collections.ObjectModel;
using WindowsTroubleShooter.Interfaces;
using System.Windows.Controls;
using WindowsTroubleShooter.Model;

namespace WindowsTroubleShooter.ViewModel
{
    public class StartViewModel 
    {
        private IssueItemViewModel _lastClickedItem;
        private Border _lastClickedBorder;       

        public StartViewModel() 
        {
           
        }

        //To cancelled/reset selection if another issueItem is clicked
        public void ListenToNextClicked(IssueItemViewModel clickedItem, Border clickedBorder)
        {
            if (_lastClickedItem != null && _lastClickedItem != clickedItem)
            {
                _lastClickedItem.Reset();
            }

            _lastClickedItem = clickedItem;
            _lastClickedBorder = clickedBorder;
        }
        

        
    }
}
