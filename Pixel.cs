using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI_V2
{
    class Pixel
    {
        private byte rouge;
        private byte vert;
        private byte bleu;
        private byte[] tab = new byte[3];


        public Pixel(byte[] tab)
        {

            this.rouge = tab[0];
            this.vert = tab[1];
            this.bleu = tab[2];

            this.tab[0] = tab[0];
            this.tab[1] = tab[1];
            this.tab[2] = tab[2];
        }
        public byte[] GetTab
        {

            get
            {
                this.tab[0] = this.rouge;
                this.tab[1] = this.vert;
                this.tab[2] = this.bleu;
                return this.tab;
            }
        }

        public byte R
        {
            get { return this.rouge; }
            set { this.rouge = value; }
        }

        public byte G
        {
            get { return this.vert; }
            set { this.vert = value; }
        }

        public byte B
        {
            get { return this.bleu; }
            set { this.bleu = value; }
        }

        public void SetWhite()
        {
            this.rouge = 255;
            this.vert = 255;
            this.bleu = 255;
        }
        public void SetBlack()
        {
            this.rouge = 0;
            this.vert = 0;
            this.bleu = 0;
        }
        public void SetGrey()
        {
            this.rouge = 127;
            this.vert = 127;
            this.bleu = 127;
        }

    }
}

