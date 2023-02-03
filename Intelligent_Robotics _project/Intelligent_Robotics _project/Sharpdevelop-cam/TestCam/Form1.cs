using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.IO.Ports;


namespace TestCam
{
    public partial class Form1 : Form
    {
        private Capture capture;
        private Image<Bgr, Byte> IMG;
        
        
        private Image<Gray, Byte> R_frame;
        private Image<Gray, Byte> G_frame;
        private Image<Gray, Byte> B_frame;
        private Image<Gray, Byte> GrayImg;
        
        private Image<Gray, Byte> B_Img_Seg;
        private Image<Gray,Byte>B_IMG_Cor;
        private Image<Gray,Byte>RED_Img_CON;
        private Image<Gray, Byte> RED_Img_Seg;
        private int Xpx,Ypx,N;
        private double Xcm,Ycm; 
        public  double Zcm;
        private double myScale;
        
        
        
        
        static SerialPort _serialPort;
        public byte []Buffer =new byte[2];
        
        
        


        
        
        public Form1()
        {
            InitializeComponent();
          _serialPort=new SerialPort();
            _serialPort.PortName="COM5";
            _serialPort.BaudRate=9600;
            _serialPort.Open();
        }
        

        private void processFrame(object sender, EventArgs e)
        {
            if (capture == null)
            {
                try
                {
                    capture = new Capture(); 
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }

            IMG = capture.QueryFrame();

            
            R_frame = IMG[2].Copy();
            G_frame = IMG[1].Copy();
            B_frame = IMG[0].Copy(); 
            
            GrayImg = IMG.Convert<Gray, Byte>();
            RED_Img_Seg=IMG.Convert<Gray,Byte>();
            RED_Img_CON=IMG.Convert<Gray,Byte>();
            B_Img_Seg=IMG.Convert<Gray,Byte>();
            B_IMG_Cor=IMG.Convert<Gray,Byte>();
            
            
            int R_th,B_th,R_corr,B_corr;
            R_th=trackBar1.Value;
            B_th=trackBar2.Value;
            R_corr=trackBar3.Value;
            B_corr=trackBar4.Value;
            

            
            for(int i=0;i< GrayImg.Width;i++)
            {
            	for (int j=0;j<GrayImg.Height;j++)
            	{if((R_frame[j,i].Intensity>=R_th)&&((B_frame[j,i].Intensity+G_frame[j,i].Intensity)<R_th  ))
            			RED_Img_Seg.Data[j,i,0]=255;
            		else
            			RED_Img_Seg.Data[j,i,0]=0;
            		
            		  
            			
            	}
            
            }
            
            for(int i=0;i<GrayImg.Width;i++)
            {
            	for (int j=0;j<GrayImg.Height;j++)
            	{
            		if(((B_frame[j,i].Intensity<B_th)&&(R_frame[j,i].Intensity)<B_th) && (G_frame[j,i].Intensity )<B_th  )
            			B_Img_Seg.Data[j,i,0]=255;
            		else
            			B_Img_Seg.Data[j,i,0]=0;
            		
            		  
            			
            	}
            
            }
            
            B_IMG_Cor = B_Img_Seg.Copy();
            
             for(int count=0;count<B_corr;count++){
           	
           	for(int i =1;i<GrayImg.Width-1;i++)
           		for(int j=1;j<GrayImg.Height-1;j++){
           	    if(B_Img_Seg[j,i].Intensity!=0)
           	    {
           			if(  (B_Img_Seg[j,i-1].Intensity==0)
           			   ||(B_Img_Seg[j,i+1].Intensity==0)
           			   ||(B_Img_Seg[j-1,i-1].Intensity==0)
           			   ||(B_Img_Seg[j-1,i].Intensity==0)
           			   ||(B_Img_Seg[j-1,i+1].Intensity==0)
           			   ||(B_Img_Seg[j+1,i+1].Intensity==0)
           			   ||(B_Img_Seg[j+1,i].Intensity==0)
           			   ||(B_Img_Seg[j+1,i-1].Intensity==0))
           			
           				B_IMG_Cor.Data[j,i,0]=0;
           			else
           				B_IMG_Cor.Data[j,i,0]=255;
           	
           	 
           	}
           		else
           			B_IMG_Cor.Data[j,i,0]=0;
           		           			           
           }
                    
           	B_IMG_Cor.CopyTo(B_Img_Seg);
            }


            
            
            
            
            
            
            

            RED_Img_CON = RED_Img_Seg.Copy();


            for(int count=0;count<R_corr;count++){
           	
           	for(int i =1;i<GrayImg.Width-1;i++)
           		for(int j=1;j<GrayImg.Height-1;j++){
           	    if(RED_Img_Seg[j,i].Intensity!=0)
           	    {
           			if(  (RED_Img_Seg[j,i-1].Intensity==0)
           			   ||(RED_Img_Seg[j,i+1].Intensity==0)
           			   ||(RED_Img_Seg[j-1,i-1].Intensity==0)
           			   ||(RED_Img_Seg[j-1,i].Intensity==0)
           			   ||(RED_Img_Seg[j-1,i+1].Intensity==0)
           			   ||(RED_Img_Seg[j+1,i+1].Intensity==0)
           			   ||(RED_Img_Seg[j+1,i].Intensity==0)
           			   ||(RED_Img_Seg[j+1,i-1].Intensity==0))
           			
           				RED_Img_CON.Data[j,i,0]=0;
           			else
           				RED_Img_CON.Data[j,i,0]=255;
           	
           	 
           	}
           		else
           			RED_Img_CON.Data[j,i,0]=0;
           		
           			
           
           
           }
           
           	RED_Img_CON.CopyTo(RED_Img_Seg);
           }
            
            

            
            
            
            
            
      
            try
            {
                
                imageBox1.Image = IMG;
                imageBox2.Image = RED_Img_CON;
                imageBox3.Image = B_IMG_Cor;
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
           
            
          double []Proj=new double[GrayImg.Width];
           
          
            
           
         
            
           
            
            
            
            
            
            
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Application.Idle += processFrame;
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Idle -= processFrame;
            button1.Enabled = true;
            button2.Enabled = false;
        }    

        private void button3_Click(object sender, EventArgs e)
        {
            IMG.Save("Image" +  ".jpg");
        }
		void Button4Click(object sender, EventArgs e)
		{
			  
   
          double []Proj=new double[GrayImg.Width];
           
            
         
        
          for (int i=0;i <GrayImg.Width;i++){
           	     double column=0;
            	
           	   
            for(int j=0;j<GrayImg.Height;j++)
            	
            		Proj[i]=column=column+(( RED_Img_CON[j,i].Intensity)/255);
            
          }
            
      
            int start=0;
            int k=0;
            int end=0;
            double  AVG=0;
            double SumProj=0;
            
            while(k<Proj.Length  && Proj[k]==0) k++;
            k+=5; 
            start=k;
            for(int i=0;i<2;i++)
            {
            	while(k<Proj.Length  && Proj[k]!=0)k++;
            	k+=5;
            	
            	while(Proj[k]==0 && (k<(GrayImg.Width-5)))k++;
            	k+=5;
            	end=k;
            	SumProj=SumProj+(end-start);
            	start=end;
            	
            
            }
            AVG=SumProj/2.0;
           
            myScale=20.0/AVG;
            
            
            textBox9.Text=myScale.ToString("0.00");
            textBox7.Text=AVG.ToString();
            textBox10.Text=IMG.Height.ToString();
            textBox11.Text=IMG.Width.ToString();
            
            
            
		
		}
		void Button5Click(object sender, EventArgs e)
		{
			 double Th1=0;
			double Th2=0;
			
		
                Th1=((int)(Th1+45));
                Th2=((int)Th2+45);
                
                Buffer[0]=(byte)Th1;
                Buffer[1]=(Byte)Th2;
                 
                _serialPort.Write(Buffer,0,2);
            
		}
		void Button6Click(object sender, EventArgs e)
		{
			
             Xpx = 0;
            Ypx = 0;
            N = 0;
            for (int i=0;i<GrayImg.Width;i++)
            for (int j=0;j<GrayImg.Height; j++)
            {
            	if(B_IMG_Cor[j,i].Intensity>128 )
                 {
                     
                     Xpx+=i;
                     Ypx+=j;
                     N++;
                 }
            }
            if (N > 0)
            {
                Xpx = Xpx/N;
                Ypx = Ypx/N;
                
                
                
                textBox12.Text=Xpx.ToString();
                textBox13.Text=Ypx.ToString();
                
                
                
                double Py= Xpx-(GrayImg.Width/2);
                double Pz= -(Ypx-GrayImg.Height/2);
                
                

                Xcm = 100;
                
                	
                Ycm = Py*myScale;
                Zcm=Pz*myScale+20;
                	

                textBox1.Text = Zcm.ToString("0.00");
                textBox2.Text = Ycm.ToString("0.00");
                textBox3.Text = N.ToString();
                
                double Th1=Math.Atan(Ycm/Xcm);
                double Th2=Math.Atan((Zcm/Ycm) *Math.Sin(Th1))*(180/Math.PI);
                
                Th1=Th1*(180/Math.PI);
                textBox4.Text=Th1.ToString();
                textBox5.Text=Th2.ToString();
                
                
                	
                Th1=(95-(int)(Th1));
                Th2=(80-(int)(Th2));
                
                Buffer[0]=(byte)Th1;
                Buffer[1]=(Byte)Th2;
                
                _serialPort.Write(Buffer,0,2);
                	
                	
                	
               
                
                
                
                
                
           
            }
            
            
	
		}
		void Button7Click(object sender, EventArgs e)
		{
			 
            
            
            
            
            
		}
		void Button8Click(object sender, EventArgs e)
		{ 
			double Th1=0;
			double Th2=0;
			
		
                Th1=((int)(Th1)+90);
                Th2=((int)Th2+93);
                
                Buffer[0]=(byte)Th1;
                Buffer[1]=(Byte)Th2;
                _serialPort.Write(Buffer,0,2);
                
		
		    
	
		}
		void TextBox8TextChanged(object sender, EventArgs e)
		{
	
		}       

        
    }
}
