
using Gama.Common.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.CommonTests
{
    public class PaginatedCollectionViewTests
    {
        private List<LookupItem> _elementos;
        private PaginatedCollectionView _paginatedCollectionView;
        private int _itemsPerPage;

        public PaginatedCollectionViewTests()
        {
            _elementos = new List<LookupItem>
            { new LookupItem { Id = 1 },
                new LookupItem { Id = 2},
                new LookupItem { Id = 3},
                new LookupItem { Id = 4},
                new LookupItem { Id = 5},
                new LookupItem { Id = 6},
                new LookupItem { Id = 7},
                new LookupItem { Id = 8},
                new LookupItem { Id = 9},
                new LookupItem { Id = 10},
                new LookupItem { Id = 11}
            };
            _itemsPerPage = 3;

            _paginatedCollectionView = new PaginatedCollectionView(_elementos, _itemsPerPage);
        }

        [Fact]
        public void ShouldInitialize()
        {
            var paginatedCollectionView = new PaginatedCollectionView(_elementos, 2);
        }

        [Fact]
        public void ShouldCalculatePageCount()
        {
            var personas = new List<LookupItem>
            { new LookupItem { Id = 1 },
                new LookupItem { Id = 2},
                new LookupItem { Id = 3},
                new LookupItem { Id = 4},
                new LookupItem { Id = 5},
                new LookupItem { Id = 6},
                new LookupItem { Id = 7},
                new LookupItem { Id = 8},
                new LookupItem { Id = 9},
            };

            var paginatedCollectionView = new PaginatedCollectionView(personas, 2);
            var expected = 5;
            Assert.Equal(expected, paginatedCollectionView.PageCount);

            paginatedCollectionView = new PaginatedCollectionView(personas, 3);
            expected = 3;
            Assert.Equal(expected, paginatedCollectionView.PageCount);

            paginatedCollectionView = new PaginatedCollectionView(personas, 12);
            expected = 1;
            Assert.Equal(expected, paginatedCollectionView.PageCount);
        }

        [Fact]
        public void ShouldAllowHavingZeroOrLessThanItemsPerPageNumberOfItems()
        {
            var personas = new List<LookupItem>
            { new LookupItem { Id = 1 },
                new LookupItem { Id = 2},
            };

            var paginatedCollectionView = new PaginatedCollectionView(personas, 3);
            var temp = paginatedCollectionView.GetItemAt(0);
            temp = paginatedCollectionView.GetItemAt(1);

            personas = new List<LookupItem>();
            paginatedCollectionView = new PaginatedCollectionView(personas, 10);
        }

        [Fact]
        public void CountShouldReturnInnerCollectionSizeIfThisSizeIsLessThanItemsPerPage()
        {
            var personas = new List<LookupItem>
            { new LookupItem { Id = 1 },
                new LookupItem { Id = 2},
            };
            var paginatedCollectionView = new PaginatedCollectionView(personas, 3);
            Assert.Equal(paginatedCollectionView.Count, 2);
        }

        [Fact]
        public void ShouldAccessThePropperIndexesOfTheInnerList()
        {
            int expectedIndex = 0;
            int expectedCurrentPage = 1;

            // Página 1
            Assert.Equal(_paginatedCollectionView.CurrentPage, expectedCurrentPage);
            Assert.Equal(_paginatedCollectionView.GetItemAt(0), _elementos[expectedIndex]);

            // Página 2
            expectedIndex += _itemsPerPage;
            expectedCurrentPage += 1;
            _paginatedCollectionView.MoveToNextPage();
            Assert.Equal(_paginatedCollectionView.CurrentPage, expectedCurrentPage);
            Assert.Equal(_paginatedCollectionView.GetItemAt(0), _elementos[expectedIndex]);

            // Página 3
            expectedIndex += _itemsPerPage;
            expectedCurrentPage += 1;
            _paginatedCollectionView.MoveToNextPage();
            Assert.Equal(_paginatedCollectionView.CurrentPage, expectedCurrentPage);
            Assert.Equal(_paginatedCollectionView.GetItemAt(0), _elementos[expectedIndex]);

            // Página 2 de nuevo
            expectedIndex -= _itemsPerPage;
            expectedCurrentPage -= 1;
            _paginatedCollectionView.MoveToPreviousPage();
            Assert.Equal(_paginatedCollectionView.CurrentPage, expectedCurrentPage);
            Assert.Equal(_paginatedCollectionView.GetItemAt(0), _elementos[expectedIndex]);

            // Página 1 de nuevo
            expectedIndex -= _itemsPerPage;
            expectedCurrentPage -= 1;
            _paginatedCollectionView.MoveToPreviousPage();
            Assert.Equal(_paginatedCollectionView.CurrentPage, expectedCurrentPage);
            Assert.Equal(_paginatedCollectionView.GetItemAt(0), _elementos[expectedIndex]);
        }

        [Fact]
        public void CurrentPageShouldntBeLessNorGreaterThanTotalNumberOfPages()
        {
            for (int i = 0; i < _paginatedCollectionView.PageCount + 10; i++)
            {
                _paginatedCollectionView.MoveToNextPage();
            }
            Assert.True(_paginatedCollectionView.CurrentPage == _paginatedCollectionView.PageCount);

            for (int i = 0; i < _paginatedCollectionView.PageCount + 20; i++)
            {
                _paginatedCollectionView.MoveToPreviousPage();
            }
            Assert.True(_paginatedCollectionView.CurrentPage == 1);
        }

        [Fact]
        public void ShouldChangeInnerCollectionCurrentPageOnNextOrPreviousPageCommand()
        {
            Assert.True(_paginatedCollectionView.Count == _itemsPerPage);

            _paginatedCollectionView.MoveToNextPage();
            _paginatedCollectionView.MoveToNextPage();
            _paginatedCollectionView.MoveToNextPage();
            Assert.True(_paginatedCollectionView.CurrentPage == 4);
            Assert.True(_paginatedCollectionView.Count == 2); // 11 - 3*3;

            _paginatedCollectionView.MoveToPreviousPage();
            Assert.True(_paginatedCollectionView.CurrentPage == 3);
            Assert.True(_paginatedCollectionView.Count == _itemsPerPage);

            _paginatedCollectionView.MoveToPreviousPage();
            _paginatedCollectionView.MoveToPreviousPage();
            Assert.True(_paginatedCollectionView.CurrentPage == 1);
            Assert.True(_paginatedCollectionView.Count == _itemsPerPage); // De nuevo en la primera página
        }
    }
}
