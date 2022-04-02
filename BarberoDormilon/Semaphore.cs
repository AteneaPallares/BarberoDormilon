using System;
using System.Collections.Generic;
using System.Text;

namespace BarberoDormilon
{
    class Semaphore
    {
        private int _bandera;
        public bool disponible;
        public int bander { set { _bandera = value; } get { return _bandera; } }
        public int sillas;
        private int pos;
        public int[] clients = { 1, 1, 1 };

        public Semaphore()
        {
            _bandera = 1;
            sillas = 0;
        }

        public void Reset()
        {
            pos = 0;
            clients[0] = 1;
            clients[1] = 1;
            clients[2] = 1;
            _bandera = 1;
            sillas = 0;
            disponible = true;
        }
        public int semWait()
        {
            int aux = 0;
            while (aux < 3 && clients[aux] != 1)
            {
                aux++;
            }

            if (_bandera == 1)
            {
                _bandera = 0;
            }
            else
            {
                if (aux < 3)
                {
                    clients[aux] = 0;
                    sillas++;
                }
            }
            return aux;
        }


        public void semSignal()
        {
            if (sillas > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (clients[i] == 0)
                    {
                        clients[i] = 1;
                        pos = i;
                        break;
                    }
                }
                sillas--;
            }
            else
            {
                _bandera = 1;
            }
        }
    }
}
