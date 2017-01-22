using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Gama.Common.CustomControls
{
    public enum SearchMode
    {
        Delayed,
        Instant,
    }

    public class SearchBox : TextBox
    {
        private Popup _popup;
        private ListBox _listBox;
        private string _textCache = "";
        private bool _suppressEvent = false;
        private static int _timeDelay = 500;
        private DispatcherTimer _searchEventDelayTimer;

        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SearchBox),
                new FrameworkPropertyMetadata(typeof(SearchBox)));
        }

        public SearchBox() : base()
        {
            _searchEventDelayTimer = new DispatcherTimer();
            _searchEventDelayTimer.Interval = SearchEventTimeDelay.TimeSpan;
            _searchEventDelayTimer.Tick += new EventHandler(OnSeachEventDelayTimerTick);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var iconBorder = GetTemplateChild("PART_SearchIconBorder") as Border;
            if (iconBorder != null)
            {
                iconBorder.MouseLeftButtonDown += new MouseButtonEventHandler(IconBorder_MouseLeftButtonDown);
                iconBorder.MouseLeftButtonUp += new MouseButtonEventHandler(IconBorder_MouseLeftButtonUp);
                iconBorder.MouseLeave += new MouseEventHandler(IconBorder_MouseLeave);
            }

            _popup = Template.FindName("PART_Popup", this) as Popup;

            _listBox = Template.FindName("PART_ListBox", this) as ListBox;
            if (_listBox != null)
            {
                _listBox.PreviewMouseDown += new MouseButtonEventHandler(listBox_MouseUp);
                _listBox.KeyDown += new KeyEventHandler(listBox_KeyDown);

                OnItemsSourceChanged(ItemsSource);
                OnItemTemplateChanged(ItemTemplate);
                OnItemContainerStyleChanged(ItemContainerStyle);
                OnItemTemplateSelectorChanged(ItemTemplateSelector);
            }
        }

        public LookupItem SelectedItem
        {
            get { return (LookupItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public int MaxCompletions
        {
            get { return (int)GetValue(MaxCompletionsProperty); }
            set { SetValue(MaxCompletionsProperty, value); }
        }

        public Duration SearchEventTimeDelay
        {
            get { return (Duration)GetValue(SearchEventTimeDelayProperty); }
            set { SetValue(SearchEventTimeDelayProperty, value); }
        }

        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public Brush LabelTextColor
        {
            get { return (Brush)GetValue(LabelTextColorProperty); }
            set { SetValue(LabelTextColorProperty, value); }
        }

        public SearchMode SearchMode
        {
            get { return (SearchMode)GetValue(SearchModeProperty); }
            set { SetValue(SearchModeProperty, value); }
        }

        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            private set { SetValue(HasTextPropertyKey, value); }
        }

        public bool IsMouseLeftButtonDown
        {
            get { return (bool)GetValue(IsMouseLeftButtonDownProperty); }
            private set { SetValue(IsMouseLeftButtonDownPropertyKey, value); }
        }

        public event RoutedEventHandler Search
        {
            add { AddHandler(SearchEvent, value); }
            remove { RemoveHandler(SearchEvent, value); }
        }

        public event RoutedEventHandler SelectResult
        {
            add { AddHandler(SelectResultEvent, value); }
            remove { RemoveHandler(SelectResultEvent, value); }
        }

        private void RaiseSearchEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(SearchEvent);
            RaiseEvent(args);
        }

        private void RaiseSelectResultEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(SelectResultEvent);
            RaiseEvent(args);
        }

        private void SetTextValueBySelection(bool moveFocus)
        {
            if (_popup != null)
            {
                InternalClosePopup();
                Dispatcher.Invoke(new Action(() => {
                    Focus();
                    if (moveFocus)
                        MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }), System.Windows.Threading.DispatcherPriority.Background);
            }

            _suppressEvent = true;
            //Text = selectedItem.DisplayMember1;
            SelectAll();
            _suppressEvent = false;
        }

        private void InternalClosePopup()
        {
            if (_popup != null)
                _popup.IsOpen = false;
        }

        private void InternalOpenPopup()
        {
            if (_popup != null && !_popup.IsOpen)
            {
                _popup.IsOpen = true;
                _listBox.SelectedIndex = -1;
            }
        }

        public void ShowPopup()
        {
            if (_listBox == null || _popup == null) InternalClosePopup();
            else if (_listBox.Items.Count == 0) InternalClosePopup();
            else InternalOpenPopup();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            HasText = Text.Trim().Length != 0;
            _searchEventDelayTimer.Stop();

            if (e.Changes.First().AddedLength == Text.Length)
                _suppressEvent = true;

            if (SearchMode == SearchMode.Delayed && !_suppressEvent && HasText)
            {
                _searchEventDelayTimer.Stop();
                _searchEventDelayTimer.Start();
            }
            _suppressEvent = false;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            SelectAll();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            var fs = FocusManager.GetFocusScope(this);
            var o = FocusManager.GetFocusedElement(fs);
            if (e.Key == Key.Escape)
            {
                InternalClosePopup();
                Focus();
            }
            else if (e.Key == Key.Down)
            {
                if (_listBox != null && o == this)
                {
                    _suppressEvent = true;
                    _listBox.Focus();
                    _suppressEvent = false;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {

            if (e.Key == Key.Escape)
                Text = "";
            else if (e.Key == Key.Return || e.Key == Key.Enter)
                RaiseSearchEvent();
            else
                base.OnKeyDown(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (_suppressEvent) return;
            if (_popup != null)
            {
                InternalClosePopup();
            }
        }

        private void IconBorder_MouseLeftButtonDown(object obj, MouseButtonEventArgs e)
        {
            IsMouseLeftButtonDown = true;
        }

        private void IconBorder_MouseLeftButtonUp(object obj, MouseButtonEventArgs e)
        {
            if (!IsMouseLeftButtonDown) return;

            if (HasText)
                RaiseSearchEvent();

            IsMouseLeftButtonDown = false;
        }

        private void IconBorder_MouseLeave(object obj, MouseEventArgs e)
        {
            IsMouseLeftButtonDown = false;
        }

        void listBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _suppressEvent = true;
            SetTextValueBySelection(moveFocus: false);
            SelectedItem = _listBox.SelectedItem as LookupItem;
            RaiseSelectResultEvent();
            _suppressEvent = false;
        }

        void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            _suppressEvent = true;
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                SetTextValueBySelection(moveFocus: false);
                SelectedItem = _listBox.SelectedItem as LookupItem;
                RaiseSelectResultEvent();
            }
            else if (e.Key == Key.Tab)
            {
                SetTextValueBySelection(moveFocus: true);
                SelectedItem = _listBox.SelectedItem as LookupItem;
                RaiseSelectResultEvent();
            }
            _suppressEvent = false;
        }

        void OnSeachEventDelayTimerTick(object o, EventArgs e)
        {
            _searchEventDelayTimer.Stop();
            if (_suppressEvent) return;

            RaiseSearchEvent();
            // La comprobación de Items.Count previene de una excepción
            // cuya raíz no he sido capaz de identificar. De todas formas,
            // téngase en cuenta que se trata de abrir el popup antes de 
            // lanzar el evento 
            if (_popup != null && _listBox != null && _listBox.Items.Count == 1)
            {
                InternalOpenPopup();
            }

            _textCache = Text ?? "";

            if (_popup != null && _textCache == "")
            {
                InternalClosePopup();
            }
            else if (_listBox != null)
            {
                if (_popup != null)
                {
                    if (_listBox.Items.Count == 0)
                        InternalClosePopup();
                    else
                        InternalOpenPopup();
                }
            }
        }

        #region Dependency Properties

        public static DependencyProperty LabelTextProperty =
            DependencyProperty.Register(
                "LabelText",
                typeof(string),
                typeof(SearchBox));

        public static DependencyProperty LabelTextColorProperty =
            DependencyProperty.Register(
                "LabelTextColor",
                typeof(Brush),
                typeof(SearchBox));

        public static DependencyProperty SearchModeProperty =
            DependencyProperty.Register(
                "SearchMode",
                typeof(SearchMode),
                typeof(SearchBox),
                new PropertyMetadata(SearchMode.Delayed));

        private static DependencyPropertyKey HasTextPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "HasText",
                typeof(bool),
                typeof(SearchBox),
                new PropertyMetadata());

        public static DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        private static DependencyPropertyKey IsMouseLeftButtonDownPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IsMouseLeftButtonDown",
                typeof(bool),
                typeof(SearchBox),
                new PropertyMetadata());

        public static DependencyProperty IsMouseLeftButtonDownProperty =
            IsMouseLeftButtonDownPropertyKey.DependencyProperty;

        public static DependencyProperty SearchEventTimeDelayProperty =
            DependencyProperty.Register(
                "SearchEventTimeDelay",
                typeof(Duration),
                typeof(SearchBox),
                new FrameworkPropertyMetadata(
                    new Duration(new TimeSpan(0, 0, 0, 0, _timeDelay)),
                    new PropertyChangedCallback(OnSearchEventTimeDelayChanged)));

        public static readonly DependencyProperty MaxCompletionsProperty =
            DependencyProperty.Register(
                "MaxCompletions",
                typeof(int),
                typeof(SearchBox),
                new UIPropertyMetadata(int.MaxValue));

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            ItemsControl.ItemTemplateSelectorProperty.AddOwner(
                typeof(SearchBox),
                new UIPropertyMetadata(null, OnItemTemplateSelectorChanged));

        private static void OnItemTemplateSelectorChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchBox actb = d as SearchBox;
            if (actb == null) return;
            actb.OnItemTemplateSelectorChanged(e.NewValue as DataTemplateSelector);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem",
                typeof(LookupItem),
                typeof(SearchBox),
                new FrameworkPropertyMetadata(null, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner(
                typeof(SearchBox),
                new UIPropertyMetadata(null, OnItemTemplateChanged));

        private static void OnItemTemplateChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchBox actb = d as SearchBox;
            if (actb == null) return;
            actb.OnItemTemplateChanged(e.NewValue as DataTemplate);
        }

        private void OnItemTemplateChanged(DataTemplate p)
        {
            if (_listBox == null) return;
            _listBox.ItemTemplate = p;
        }

        public static readonly DependencyProperty ItemContainerStyleProperty =
            ItemsControl.ItemContainerStyleProperty.AddOwner(
                typeof(SearchBox),
                new UIPropertyMetadata(null, OnItemContainerStyleChanged));

        private static void OnItemContainerStyleChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchBox actb = d as SearchBox;
            if (actb == null) return;
            actb.OnItemContainerStyleChanged(e.NewValue as Style);
        }

        private void OnItemContainerStyleChanged(Style p)
        {
            if (_listBox == null) return;
            _listBox.ItemContainerStyle = p;
        }

        static void OnSearchEventTimeDelayChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var stb = o as SearchBox;
            if (stb != null)
            {
                stb._searchEventDelayTimer.Interval = ((Duration)e.NewValue).TimeSpan;
                stb._searchEventDelayTimer.Stop();
            }
        }

        private void OnItemTemplateSelectorChanged(DataTemplateSelector p)
        {
            if (_listBox == null) return;
            _listBox.ItemTemplateSelector = p;
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            ItemsControl.ItemsSourceProperty.AddOwner(
                typeof(SearchBox),
                new UIPropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchBox actb = d as SearchBox;
            if (actb == null) return;
            actb.OnItemsSourceChanged(e.NewValue as IEnumerable);
        }

        protected void OnItemsSourceChanged(IEnumerable itemsSource)
        {
            if (_listBox == null)
                return;

            if (itemsSource is ListCollectionView)
                _listBox.ItemsSource = new LimitedListCollectionView((IList)((ListCollectionView)itemsSource).SourceCollection);
            else if (itemsSource is CollectionView)
                _listBox.ItemsSource = new LimitedListCollectionView(((CollectionView)itemsSource).SourceCollection);
            else if (itemsSource is IList)
                _listBox.ItemsSource = new LimitedListCollectionView((IList)itemsSource);
            else
            {
                if (itemsSource == null)
                    itemsSource = new List<string>();

                _listBox.ItemsSource = new LimitedCollectionView(itemsSource);
            }

            if (_listBox.Items.Count == 0)
                InternalClosePopup();
        }

        #endregion

        public static readonly RoutedEvent SearchEvent =
            EventManager.RegisterRoutedEvent(
                "Search",
                RoutingStrategy.Direct,
                typeof(RoutedEventHandler),
                typeof(SearchBox));

        public static readonly RoutedEvent SelectResultEvent =
            EventManager.RegisterRoutedEvent(
                "SelectResult",
                RoutingStrategy.Direct,
                typeof(RoutedEventHandler),
                typeof(SearchBox));
    }
}
