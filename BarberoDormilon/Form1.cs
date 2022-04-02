using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace BarberoDormilon
{
    public partial class Form1 : Form
    {
        private static Semaphore semaphore;
        public int waitTime = 1000;
        private bool open = false;
        private int cortes = 0;
        private bool cutting = false;
        private bool comming = true;
        System.Timers.Timer waitT;
        System.Timers.Timer workTime;
        System.Timers.Timer clienteTime;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            semaphore = new Semaphore();
            waitT = new System.Timers.Timer(waitTime);
            waitT.Elapsed += OnTimeEvent;
            workTime = new System.Timers.Timer(1000);
            workTime.Elapsed += OnWorkEvent;
            clienteTime = new System.Timers.Timer(4000);
            clienteTime.Elapsed += OnclienteEvent;
            Reset();
        }

        private void clear()
        {
            for (int i = 0; i < 3; i++)
            {
                clearElement(i);
            }
        }
        private void fillElement(int k)
        {
            int j = k / table.ColumnCount;
            int i = k - (j * table.ColumnCount);
            PictureBox pB = table.GetControlFromPosition(i, j) as PictureBox;
            if (pB == null)
            {
                pB = new PictureBox
                {
                    Size = MaximumSize,
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                pB.Image = wait.Image;
                table.Controls.Add(pB, i, j);
            }
            pB.Image = wait.Image;
        }
        public void clearElement(int k)
        {
            int j = k / table.ColumnCount;
            int i = k - (j * table.ColumnCount);
            PictureBox pB = table.GetControlFromPosition(i, j) as PictureBox;
            if (pB == null)
            {
                pB = new PictureBox
                {
                    Size = MaximumSize,
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                pB.Image = free.Image;
                table.Controls.Add(pB, i, j);
            }
            pB.Image = free.Image;
        }

        private void Reset()
        {
            cutting = false;
            comming = false;
            cortes = 0;
            semaphore.Reset();
            button1.Text = "Cerrar Puerta";
            button1.BackColor = Color.Brown;
            clienteLbl.Text = "";
            barberLbl.Text = "Durmiendo";
            barberPB.Image = sleepPB.Image;
            doorPB.Image = door.Image;
            label3.Text = "Barbero";
            chairsLbl.Text = "0";
            clear();
            start();
        }

        private void start()
        {
            open = true;
            Random g = new Random();
            int clienteW = g.Next(2, 3);
            clienteW = clienteW * 1000;
            clienteTime.Interval = clienteW;
            clienteTime.Start();
        }



        private void Newcliente()
        {

            int ax = 0;

            while (!semaphore.disponible)
            {
                ax++;
            }
            semaphore.disponible = false;
            int c = semaphore.semWait();
            semaphore.disponible = true;
            comming = true;
            this.BeginInvoke(new MethodInvoker(delegate
            {
                doorPB.Image = client.Image;
                if (c < 3)
                {
                    clienteLbl.Text = "Ingreso";
                }
                else
                {
                    clienteLbl.Text = "Me retiro";
                }

            }));
            if (waitT.Enabled)
            {
                waitT.Stop();
                waitT.Start();
            }
            else
            {
                waitT.Start();
            }
            while (comming)
            {
                ax++;
            }

            this.BeginInvoke(new MethodInvoker(delegate
            {

                chairsLbl.Text = semaphore.sillas.ToString();

                if (semaphore.bander == 0)
                {
                    for(int i = 0; i < semaphore.sillas; i++)
                    {
                        fillElement(i);
                    }
                }

            }));


            if (c < 3)
            {

                while (semaphore.clients[c] == 0)
                {
                    ax++;
                }
                if (open)
                {

                    CutHair();
                    while (!semaphore.disponible)
                    {
                        ax++;
                    }
                    semaphore.disponible = false;
                    semaphore.semSignal();
                    semaphore.disponible = true;
                    if (semaphore.bander == 1)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            barberLbl.Text = "Durmiendo";
                            barberPB.Image = sleepPB.Image;
                        }));

                    }
                }

            }

        }

        private void CutHair()
        {
            cortes++;
            cutting = true;
            Random rc = new Random();
            int cutTime = rc.Next(2, 4);
            cutTime = cutTime * 1000;
            workTime.Interval = cutTime;
            if (workTime.Enabled)
            {
                workTime.Stop();
                workTime.Start();
            }
            else
            {
                workTime.Start();

            }
            this.BeginInvoke(new MethodInvoker(delegate
            {
                chairsLbl.Text = semaphore.sillas.ToString();
                for (int i = semaphore.sillas; i < 3; i++)
                {
                    clearElement(i);
                    label3.Text = "Barbero:   "+cortes.ToString()+" cortes";
                }
                this.barberPB.Image = work.Image;
                this.barberLbl.Text = "Trabajando";
            }));

            int x = 0;
            while (cutting)
            {
                x++;
            }

        }

        private void finishBtn_Click(object sender, EventArgs e)
        {

            open = false;
            if (waitT != null && waitT.Enabled)
            {
                waitT.Stop();
            }
            if (workTime != null && workTime.Enabled)
            {
                workTime.Stop();
            }
            if (clienteTime != null && clienteTime.Enabled)
            {
                clienteTime.Stop();
            }

            Reset();

        }

        private void OnTimeEvent(object sender, ElapsedEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                this.doorPB.Image = door.Image;
                this.clienteLbl.Text = "";
            }));
            comming = false;
            waitT.Stop();
        }

        private void OnWorkEvent(object sender, ElapsedEventArgs e)
        {
            cutting = false;
            workTime.Stop();
        }

        private void OnclienteEvent(object sender, ElapsedEventArgs e)
        {
            Random bv = new Random();
            int ti = bv.Next(2, 3);
            Thread newcliente;
            newcliente = new Thread(new ThreadStart(Newcliente));
            newcliente.Start();
            clienteTime.Stop();
            clienteTime.Interval = ti * 1000;
            clienteTime.Start();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Cerrar Puerta")
            {
                if (clienteTime.Enabled)
                {
                    clienteTime.Stop();
                }
                button1.Text = "Abrir Puerta";
            }
            else
            {
                open = true;
                Random g = new Random();
                int clienteW = g.Next(2, 3);
                clienteW = clienteW * 1000;
                clienteTime.Interval = clienteW;
                clienteTime.Start();
                button1.Text = "Cerrar Puerta";
            }
          
        }
    }
}
