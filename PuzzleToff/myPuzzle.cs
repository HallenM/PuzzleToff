using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace Game
{
    public partial class myPuzzle : Form
    {
        public myPuzzle()
        {
            InitializeComponent();
        }

        // Проигрыватель звуков
        WindowsMediaPlayer WMP;
        // Объект хранения картинки
        Bitmap picture = null;
        // Массив объектов квадратов для хранения сегментов картинки
        PictureBox[] PB = null;

        // Длина стороны в квадратах
        int lengthSquare = 3;
        // Таймер
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        private void myPuzzle_Load(object sender, EventArgs e)
        {
            puzzleTimer.Text = "00:00:00";
            WMP = new WindowsMediaPlayer();
            WMP.URL = "C:\\Users\\HallenM\\Desktop\\НГТУ и всё связанное с ним\\ЭСКиТП\\Моя игра\\PuzzleToff\\PuzzleToff\\music.mp3";
            WMP.controls.play();
        }

        void CreatePictureField()
        {
            // Удалим предыдущий массив, чтобы создать новый
            if (PB != null)
            {
                for (int i = 0; i < PB.Length; i++)
                {
                    PB[i].Dispose();
                }
                PB = null;
            }

            int numb = lengthSquare;

            // Создаём массив квадратов размером dim
            PB = new PictureBox[numb * numb];

            // Вычислим габаритные размеры квадратов мозаики
            int w = panelPuzzle.Width / numb;
            int h = panelPuzzle.Height / numb;

            // Счётчик квадратов по координате Х в одном ряду и по координате Y в одном столбце
            int countX = 0;
            int countY = 0;
            for (int i = 0; i < PB.Length; i++)
            {   // Непосредственное создание квадрата, элемента массива
                PB[i] = new PictureBox();

                // Размеры и координаты размещения созданного квадрата
                PB[i].Width = w;
                PB[i].Height = h;
                PB[i].Left = 0 + countX * PB[i].Width;
                PB[i].Top = 0 + countY * PB[i].Height;

                // Запомним начальные координаты прямоугольника для восстановления 
                // перемешанной картинки, определения полной сборки картинки
                Point pt = new Point();
                pt.X = PB[i].Left;
                pt.Y = PB[i].Top;
                // Сохраним координаты в свойстве Tag каждого квадрата
                PB[i].Tag = pt;

                // Считаем прямоугольники по рядам и столбцам
                countX++;
                if (countX == numb)
                {
                    countX = 0;
                    countY++;
                }

                // Размещение квадратов в panelPuzzle
                PB[i].Parent = panelPuzzle;
                // Нет границ
                PB[i].BorderStyle = BorderStyle.None;
                // Размеры картинки будут подгоняться под размеры прямоугольника
                PB[i].SizeMode = PictureBoxSizeMode.StretchImage;
                // Гарантируем видимость квадрата
                PB[i].Show();

                // Для всех квадратов массива событие клика мыши будет обрабатываться 
                // в одной и той же функции, для удобства
                // вычисления координат прямоугольников в одном месте
                PB[i].Click += new EventHandler(Game);
            }
            // Раскидываем картинку на сегменты и рисуем каждый сегмент
            DrawRect();
        }

        void DrawRect()
        {

            if (picture == null) return;
            int countX = 0;
            int countY = 0;
            int num = lengthSquare;
            for (int i = 0; i < PB.Length; i++)
            {
                int w = picture.Width / num;
                int h = picture.Height / num;
                PB[i].Image = picture.Clone(new RectangleF(countX * w, countY * h, w, h), picture.PixelFormat);
                countX++;
                if (countX == lengthSquare)
                {
                    countX = 0;
                    countY++;
                }

            }
        }

        void Game(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;

            if (puzzleTimer.Text == "00:00:00")
                timer.Start();

            for (int i = 0; i < PB.Length; i++)
            {
                // Определим пустое место на области рисования картинки
                if (PB[i].Visible == false)
                {
                    // Проверим кликнутый квадрат, если у него совпадают координаты по X или Y
                    // (квадраты по диагонали не рассматриваются), но при этом он на ближайшем расстоянии 
                    // от пустого прямоугольника (отбрасываем квадраты ч/з квадрат от пустого)
                    if (
                        (pb.Location.X == PB[i].Location.X &&
                         Math.Abs(pb.Location.Y - PB[i].Location.Y) == PB[i].Height)
                        ||
                        (pb.Location.Y == PB[i].Location.Y &&
                         Math.Abs(pb.Location.X - PB[i].Location.X) == PB[i].Width)
                        )
                    {
                        // Меняем местами пустой и кликнутый квадраты
                        Point pt = PB[i].Location;
                        PB[i].Location = pb.Location;
                        pb.Location = pt;

                        // После каждого хода проверка на полную сборку картинки
                        //============Проверка на выигрыш============//
                        // Выход из функции,если хотя бы у одного квадрата не совпали
                        // реальные и первичные координаты
                        for (int j = 0; j < PB.Length; j++)
                        {
                            Point point = (Point)PB[j].Tag;
                            if (PB[j].Location != point)
                            {
                                return;
                            }
                        }

                        // Если у всех совпали - пазл собран
                        for (int m = 0; m < PB.Length; m++)
                        {
                            PB[m].Visible = true;
                            // Убираем обрамление квадратов
                            PB[m].BorderStyle = BorderStyle.None;
                        }

                        timer.Stop();
                        MessageBox.Show("Поздравляем! Пазл был собран за " + timer.Elapsed.ToString().Remove(8), "PuzzleToff", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        DialogResult res = MessageBox.Show("Желаете повторить?", "PuzzleToff", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                        if (res == DialogResult.Yes)
                        {
                            puzzleTimer.Text = "00:00:00";
                            buttonShuffle_Click(sender, e);
                        }
                    }

                    break;
                }
            }
        }

        // Открытие диалогового окна выбора файла
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // Создание диалогового окна для выбора файла
            OpenFileDialog openDialog = new OpenFileDialog();
            // Формат загружаемого файла
            openDialog.Filter = "Image Files(*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
            // Индекс фильтра, выбранного в настоящий момент в диалоговом окне файла
            openDialog.FilterIndex = 1;
            // Восстанавливаем текущую папку перед закрытием
            openDialog.RestoreDirectory = true;
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                // Загружаем выбранную картинку
                picture = new Bitmap(openDialog.FileName);
                // Позволяет вписать картинку в маленькое поле
                this.pictureBoxEndPicture.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBoxEndPicture.Image = picture;
                pictureBoxEndPicture.Invalidate();
                // Создаём новую область прорисовки
                CreatePictureField();
            }
        }

        // Выход из игры
        private void buttonQuit_Click(object sender, EventArgs e)
        {
            FormClosingEventArgs ee = e as FormClosingEventArgs;
            DialogResult quit = MessageBox.Show("Вы действительно хотите выйти?", "PuzzleToff", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (sender as Button != buttonQuit && quit == DialogResult.No) ee.Cancel = true;
            if (sender as Button == buttonQuit && quit == DialogResult.Yes) Application.Exit();
        }

        // Открываем настройки приложения
        private void toolStripButtonSetting_Click(object sender, EventArgs e)
        {
            Settings lengthSq = new Settings();
            lengthSq.lengthSquare = lengthSquare;
            if (lengthSq.ShowDialog() == DialogResult.OK)
            {
                lengthSquare = lengthSq.lengthSquare;
                CreatePictureField();
            }
        }

        // Восстанавливаем картинку соответсвенно первичным координатам
        private void toolStripButtonRestore_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < PB.Length; i++)
            {
                Point pt = (Point)PB[i].Tag;
                PB[i].Location = pt;
                PB[i].Visible = true;
            }
        }

        // Переешиваем картинку, хаотично меняем их координаты
        private void buttonShuffle_Click(object sender, EventArgs e)
        {
            if (picture == null) return;

            // Создаём объект генерирования псевослучайных чисел,
            // для различного набора случайных чисел инициализацию
            // объекта Random производим от счетчика количества
            // миллисекунд прошедших со времени запуска операционной системы
            Random rand = new Random(Environment.TickCount);
            int r = 0;
            for (int i = 0; i < PB.Length; i++)
            {
                PB[i].Visible = true;
                r = rand.Next(0, PB.Length);
                Point ptR = PB[r].Location;
                Point ptI = PB[i].Location;
                PB[i].Location = ptR;
                PB[r].Location = ptI;
                PB[i].BorderStyle = BorderStyle.FixedSingle;
            }

            // Случайным образом выбираем пустой прямоугольник,
            // делаем его невидимым
            r = rand.Next(0, PB.Length);
            PB[r].Visible = false;

            timer.Reset();
            puzzleTimer.Text = "00:00:00";
        }

        // Управление таймером
        private void UpdateTimer(object sender, EventArgs e)
        {
            if (timer.Elapsed.ToString() != "00:00:00")
                puzzleTimer.Text = timer.Elapsed.ToString().Remove(8);
            // Видимость кнопки Пауза
            if (timer.Elapsed.ToString() == "00:00:00")
                buttonPause.Enabled = false;
            else
                buttonPause.Enabled = true;
        }

        // Пауза/возобновление игры
        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (buttonPause.Text == "Пауза")
            {
                timer.Stop();
                buttonPause.Text = "Продолжить";
                panelPuzzle.Visible = false;
            }
            else
            {
                timer.Start();
                buttonPause.Text = "Пауза";
                panelPuzzle.Visible = true;
            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            Help form = new Help();
            form.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            About inf = new About();
            inf.Show();
        }
    }
}
