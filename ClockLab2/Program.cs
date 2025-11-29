using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


using WinApp = System.Windows.Forms.Application;
using WinTimer = System.Windows.Forms.Timer;
using WinFont = System.Drawing.Font;

namespace ClockLab2
{
    // Старт программы при запуске
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            WinApp.EnableVisualStyles();
            WinApp.SetCompatibleTextRenderingDefault(false);
            WinApp.Run(new ClockForm());
        }
    }

    public class ClockForm : Form
    {
        private readonly WinTimer _timer;
        // Оформление окна и создание таймера
        public ClockForm()
        {
            Text = "Analog Clock";
            BackColor = Color.FromArgb(5, 10, 25);
            DoubleBuffered = true;
            ClientSize = new Size(400, 400);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            _timer = new WinTimer();
            _timer.Interval = 1000;
            _timer.Tick += (s, e) => Invalidate();
            _timer.Start();
        }
        // Отрисовка самих часов
        protected override void OnPaint(PaintEventArgs e)
        {
            // Берется объект для отрисовки и назначение констант
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            // Константы для обозначения ширины и высоты циферблата
            int w = ClientSize.Width;
            int h = ClientSize.Height;
            // Константы для правильного расположения циферблата относительно времени и даты 
            int bottomMargin = 40;
            int usableHeight = h - bottomMargin;
            // Константы для расположения круга, определения его размеров и т.д.
            int size = Math.Min(w, usableHeight) - 40;
            int radius = size / 2;
            int cx = w / 2;
            int cy = usableHeight / 2 + 10;
            Rectangle dialRect = new Rectangle(cx - radius, cy - radius, radius * 2, radius * 2);
            // Настройка и отрисовка циферблата
            using var dialBrush = new SolidBrush(Color.FromArgb(15, 25, 50));
            using var borderPen = new Pen(Color.FromArgb(0, 120, 230), 4);
            using var hourMarkPen = new Pen(Color.FromArgb(0, 180, 255), 3);
            using var minuteMarkPen = new Pen(Color.FromArgb(0, 90, 180), 1);
            using var numberBrush = new SolidBrush(Color.White);
            using WinFont numberFont = new WinFont("Segoe UI", 12, FontStyle.Bold);

            g.FillEllipse(dialBrush, dialRect);
            g.DrawEllipse(borderPen, dialRect);

            // Расположение минутных и часовых делений на циферблате
            for (int i = 0; i < 60; i++)
            {
                double angle = i * 6 * Math.PI / 180.0;
                bool isHourMark = (i % 5 == 0);

                int innerR = radius - (isHourMark ? 18 : 10);
                int outerR = radius - 4;

                int x1 = cx + (int)(Math.Cos(angle) * innerR);
                int y1 = cy + (int)(Math.Sin(angle) * innerR);
                int x2 = cx + (int)(Math.Cos(angle) * outerR);
                int y2 = cy + (int)(Math.Sin(angle) * outerR);

                g.DrawLine(isHourMark ? hourMarkPen : minuteMarkPen, x1, y1, x2, y2);
            }

            // Расположение цифр от 1 до 12 на циферблате
            for (int i = 1; i <= 12; i++)
            {
                double angle = (i * 30 - 90) * Math.PI / 180.0;
                int numR = radius - 30;

                string text = i.ToString();
                SizeF sizeF = g.MeasureString(text, numberFont);

                float nx = cx + (float)(Math.Cos(angle) * numR) - sizeF.Width / 2;
                float ny = cy + (float)(Math.Sin(angle) * numR) - sizeF.Height / 2;

                g.DrawString(text, numberFont, numberBrush, nx, ny);
            }

            // Настройка стрелок часов
            DateTime now = DateTime.Now;

            double hour = now.Hour % 12 + now.Minute / 60.0;
            double minute = now.Minute + now.Second / 60.0;
            double second = now.Second;
            // Преобразование времени в угол для корректного перемещения стрелок
            double hourAngle = (hour * 30 - 90) * Math.PI / 180.0;
            double minuteAngle = (minute * 6 - 90) * Math.PI / 180.0;
            double secondAngle = (second * 6 - 90) * Math.PI / 180.0;
            // Определение длины стрелок
            int hourLen = (int)(radius * 0.5);
            int minuteLen = (int)(radius * 0.72);
            int secondLen = (int)(radius * 0.82);
            // Формирование стрелки и ее расположение в круге
            Point hourEnd = new(
                cx + (int)(Math.Cos(hourAngle) * hourLen),
                cy + (int)(Math.Sin(hourAngle) * hourLen));

            Point minuteEnd = new(
                cx + (int)(Math.Cos(minuteAngle) * minuteLen),
                cy + (int)(Math.Sin(minuteAngle) * minuteLen));

            Point secondEnd = new(
                cx + (int)(Math.Cos(secondAngle) * secondLen),
                cy + (int)(Math.Sin(secondAngle) * secondLen));
            // Создание внешнего вида стрелок
            using var hourPen = new Pen(Color.FromArgb(0, 200, 255), 6) { EndCap = LineCap.Round };
            using var minutePen = new Pen(Color.FromArgb(0, 140, 255), 4) { EndCap = LineCap.Round };
            using var secondPen = new Pen(Color.FromArgb(255, 80, 80), 2);
            using var centerBrush = new SolidBrush(Color.FromArgb(0, 200, 255));

            // Отрисовка часовой стрелки
            g.DrawLine(hourPen, cx, cy, hourEnd.X, hourEnd.Y);
            // Отрисовка минутной стрелки
            g.DrawLine(minutePen, cx, cy, minuteEnd.X, minuteEnd.Y);
            // Отрисовка секундной стрелки
            g.DrawLine(secondPen, cx, cy, secondEnd.X, secondEnd.Y);

            // Отрисовка центральной точки часов
            int cR = 6;
            g.FillEllipse(centerBrush, cx - cR, cy - cR, cR * 2, cR * 2);

            // Отображение даты и времени снизу циферблата
            string timeText = now.ToString("HH:mm:ss");
            string dateText = now.ToString("dd.MM.yyyy");
            string bottomText = $"{timeText}   {dateText}";
            using WinFont digitalFont = new WinFont("Segoe UI", 11, FontStyle.Bold);
            SizeF tSize = g.MeasureString(bottomText, digitalFont);
            float tx = cx - tSize.Width / 2;
            float ty = ClientSize.Height - tSize.Height - 8;
            g.DrawString(bottomText, digitalFont, numberBrush, tx, ty);

        }
    }
}




