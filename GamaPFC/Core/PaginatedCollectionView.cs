using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Core
{
    public class PaginatedCollectionView : CollectionView
    {
        private readonly IList _innerList;
        private int _itemsPerPage;
        private int _currentPage;
        private int _pageCount;
        private int _count;

        private int _startIndex => (_currentPage - 1) * _itemsPerPage;

        public PaginatedCollectionView(IList innerList, int itemsPerPage)
            : base(innerList)
        {
            _innerList = innerList;
            _currentPage = 1;
            _pageCount = (_innerList.Count + itemsPerPage - 1) / itemsPerPage;
            ItemsPerPage = itemsPerPage;
        }

        public int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set
            {
                if (_itemsPerPage != value)
                {
                    _itemsPerPage = value;
                    CurrentPage = 1;
                    _count = Math.Min(_itemsPerPage, _innerList.Count);
                    PageCount = (_innerList.Count + _itemsPerPage - 1) / _itemsPerPage;
                    Refresh();
                }
            }
        }

        public int InnerCount => _innerList.Count;

        public override int Count => _count;

        public int PageCount
        {
            get { return _pageCount; }
            set
            {
                _pageCount = value == 0 ? 1 : value;
                OnPropertyChanged(new PropertyChangedEventArgs("PageCount"));
            }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                if (_currentPage < _pageCount)
                {
                    _count = Math.Min(_itemsPerPage, _innerList.Count);
                }
                else if (_currentPage == _pageCount)
                {
                    _count = _innerList.Count - (_pageCount - 1) * _itemsPerPage;
                }
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentPage"));
            }
        }

        public override object GetItemAt(int index)
        {
            var offset = index % _itemsPerPage;
            var finalIndex = offset + _startIndex;

            return _innerList[finalIndex];
        }

        public void MoveToNextPage()
        {
            if (_currentPage < PageCount)
            {
                CurrentPage += 1;
                Refresh();
            }
        }

        public void MoveToPreviousPage()
        {
            if (_currentPage > 1)
            {
                CurrentPage -= 1;
                Refresh();
            }
        }
    }
}
