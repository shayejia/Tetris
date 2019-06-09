using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tetris : MonoBehaviour
{

    // Use this for initialization
    float tmpdowntime = 0.5f;   //下落时间间隔
    float tmplifttime = 0;      //左右移动
    float gameTime;             //游戏时间

    public GameObject[] nowSquart;     //当前方块
    public GameObject[]nextSquart;      //下一个方块
    public GameObject TipsWin;  //提示窗口
    public Text scoretext;      //分数显示

    int score = 0;
    int nowSquartType = 0;      //当前方块种类
    int nowSquartRotType = 0;       //当前方块方向
    int nowSquartClour = 0;         //当前方块颜色
    int nextSquartType = 0;         //下一个方块的种类
    int nextSquartRotType = 0;      //下一个方块的方向
    int nextSquartClour = 0;         //下一个方块颜色


    int[,] myArray=new int [25,10];

    int[] myRow = new int[5];       //每个小方块的x坐标
    int[] myCol = new int[5];       //每个小方块的y坐标

    List<List<GameObject>> allSquart=new List<List<GameObject>>();      //所有方块
    List<int> reList = new List<int>();         //需要移除的行数
    
             //是否可以移动
    bool[] moveTypeL = new bool[4];
    bool[] moveTypeR = new bool[4];
    bool[] moveTypeD = new bool[4];
    void Awake()
    {
        RangeNextSquart();
    }
    void Start()
    {

        gameTime = Time.time;
        Into();             //初始化信息
        NextSquart();      //创建方块

    }

    // Update is called once per frame
    void Update()
    {
        if (nowSquart != null)
        {
            SquareMove();               //方块移动
        }
    }

    void IntoMoveTypeOp()       //重置标记开关
    {
        for (int i = 0; i < 4; i++)
        {
            moveTypeD[i] = false;
            moveTypeL[i] = false;
            moveTypeR[i] = false;
        }
    }
    void Into()
    {
        
       for(int i=0;i<25;i++)                //数组初始化
        {
            for(int j=0;j<10;j++)
            {
                myArray[i, j] = 0;
            }
        }
        for(int i=0;i<25;i++)                           //方块容器分配空间
        {
           List<GameObject>list= new List<GameObject>();
            for (int j=0;j<10;j++)
            {
                GameObject obj = null;
                list.Add(obj);
            }

            allSquart.Add(list);
        }         
        IntoNowSquart();    

    }

    void DeleatNext()       //删除之前的下一个方块的提示
    { 
        for(int i=0;i<4;i++)
        {
            if (nextSquart[i] != null)
            {
                Destroy(nextSquart[i]);
            }
        }
    }
    void RangeNextSquart()      //随机下一个方块的颜色，形状，方向
    {
        nextSquartType = Random.Range(0, 7);
        //nextSquartType = 2;           
        nextSquartRotType = Random.Range(0, 4);
        //nextSquartRotType = 1;
        nextSquartClour = Random.Range(1, 6);
    }

    void IntoNowSquart()        //初始化当前方块信息
    {
        nowSquartClour = nextSquartClour;
        nowSquartRotType = nextSquartRotType;
        nowSquartType = nextSquartType;
    }
    void NextSquart()               //创建的方块
    {
        DeleatNext();
        IntoNowSquart();      
        CreatSquart(nowSquart, 20, 5, nowSquartClour, nowSquartType, nowSquartRotType);  //创建游戏中的方块
        RangeNextSquart();      //随机下一个方块
        CreatSquart(nextSquart, 13, 17, nextSquartClour, nextSquartType, nextSquartRotType);    //创建下一个方块提示
        
    }
    
    void CreatSquart(GameObject[]squart,int row,int col,int clour,int type,int rot)      //创建方块(对象，坐标，颜色，形状，方向)
    {
        if(squart==nowSquart)                               //初始化坐标信息
        {
            myRow[4] = row;
            myCol[4] = col;
        }
       
        for(int i=0; i<4; i++)                          //依次创建四个小方块
        {
            if (squart == nowSquart)
            {
                myRow[i] = myRow[4];
                myCol[i] = myCol[4];
            }
            squart[i] = GameObject.Instantiate(Resources.Load<GameObject>("Cube"+clour.ToString()));          
            squart[i].transform.position = new Vector3(col, row, 0);         
        }           
        SetPostion(squart,type,rot);    //设置小方块初始位置
    }

    void SetPostion(GameObject[]squart,int type,int rot)  //设置方块   (对象，形状，方向)
    {
        switch (type)
        {
            case 0:                 //长条
                CreatISquare(squart,rot);
                break;
            case 1:                 //正方形
                CreatOSquare(squart, rot);
                break;
            case 2:                 //土字形
                CreatTSquare(squart, rot);
                break;
            case 3:         // L形
                CreatLSquare(squart, rot);
                break;
            case 4:         //反L形
                CreatJSquare(squart, rot);
                break;
            case 5:     //s形
                CreatOSquare(squart, rot);
                break;
            case 6:         //Z形
                CreatZSquare(squart, rot);
                break;
        }

    }

    void SkewingSquare(GameObject[]obj,int index,int x,int y)        //移动每个小方块的位置
    {
        obj[index].transform.Translate(Vector3.right * x);          //移动方块的位置
        obj[index].transform.Translate(Vector3.up * y);
        if (obj == nowSquart)
        {        
            myRow[index] += y;                  //更新方块在数组中的坐标
            myCol[index] += x;
        }
        
    }

    void ResetSquare()          //重置方块
    { 
        for(int i = 0; i < 4; i++)                  //将所有小方块移动到中心位置
        {
            myRow[i] = myRow[4];
            myCol[i] = myCol[4];
            nowSquart[i].transform.position = new Vector3(myCol[4], myRow[4], 0);
        }
    }
    void CreatISquare(GameObject[]obj,int rot)     //创建I形方块     （对象，方向）
    {
        if(rot==0||rot==2)            //对应的两个方向 
        {
            SkewingSquare( obj,1, 1, 0);
            SkewingSquare(obj,2, 2, 0);
            SkewingSquare(obj,3, 3, 0);
        }
        else
        {
            SkewingSquare(obj,1, 0, 1);
            SkewingSquare(obj,2, 0, 2);
            SkewingSquare(obj,3, 0, 3);
        }
    }

    void CreatOSquare(GameObject[] obj, int rot) //创建O形方块  只有一种方向
    {
        SkewingSquare(obj,0, -1, 0);
        SkewingSquare(obj,2, -1, 1);
        SkewingSquare(obj,3, 0, 1);
    }

    void CreatTSquare(GameObject[] obj, int rot)         //创建T形方块
    {
        switch(rot)                //对应四种方向
        {
            case 0:
                SkewingSquare(obj,0, -1, 0);
                SkewingSquare(obj,2, 1, 0);
                SkewingSquare(obj,3, 0, 1);
                break;
            case 1:
                SkewingSquare(obj,1, 0, 1);
                SkewingSquare(obj,2, 1, 1);
                SkewingSquare(obj,3, 0, 2);
                break;
            case 2:
                SkewingSquare(obj,1, -1, 1);
                SkewingSquare(obj,2, 0, 1);
                SkewingSquare(obj,3, 1, 1);
                break;
            case 3:
                SkewingSquare(obj,1, -1, 1);
                SkewingSquare(obj,2, 0, 1);
                SkewingSquare(obj,3, 0, 2);
                break;
        }
    }

    void CreatLSquare(GameObject[] obj, int rot)     //创建L形方块            
    {
        switch(rot)        //对应四种方向
        {
            case 0:
                SkewingSquare(obj,0, -1, 0);
                SkewingSquare(obj,2, -1, 1);
                SkewingSquare(obj,3, -1, 2);
                break;
            case 1:
                SkewingSquare(obj,0, -1, 0);
                SkewingSquare(obj,1, -1, 1);
                SkewingSquare(obj,2, 0, 1);
                SkewingSquare(obj,3, 1, 1);
                break;
            case 2:
                SkewingSquare(obj,1, 0, 1);
                SkewingSquare(obj,2, 0, 2);
                SkewingSquare(obj,3, -1, 2);
                break;
            case 3:
                SkewingSquare(obj,0, -1, 0);             
                SkewingSquare(obj,2, 1, 0);
                SkewingSquare(obj,3, 1, 1);
                break;
        }
    }

    void CreatJSquare(GameObject[] obj, int rot)             //创建J形方块
    {
        
        switch(rot)       //对应四种方向
        {
            case 0:
                SkewingSquare(obj,0, -1, 0);
                SkewingSquare(obj,2, 0, 1);
                SkewingSquare(obj,3, 0, 2);
                break;
            case 1:
                SkewingSquare(obj,0, -1, 0);
                SkewingSquare(obj,2, 1, 0);
                SkewingSquare(obj,3, -1, 1);
                break;
            case 2:
                SkewingSquare(obj,1, 0, 1);
                SkewingSquare(obj,2, 0, 2);
                SkewingSquare(obj,3, 1, 2);
                break; 
            case 3:
                SkewingSquare(obj,0, 1, 0);
                SkewingSquare(obj,1, -1, 1);
                SkewingSquare(obj,2, 0, 1);
                SkewingSquare(obj,3, 1, 1);
                break;
        }
    }

    void CreatSSquare(GameObject[] obj, int rot)     //创建S形方块
    {
        if(rot==0||rot==2)    //对应两种方向
        {
            SkewingSquare(obj,0, -1, 0);
            SkewingSquare(obj,2, 0, 1);
            SkewingSquare(obj,3, 1, 1);
        }
        else
        {
            SkewingSquare(obj,1, -1, 1);
            SkewingSquare(obj,2, 0, 1);
            SkewingSquare(obj,3, -1, 2);
        }
    }

    void CreatZSquare(GameObject[] obj, int rot)     //创建Z形方块
    {
        if(rot==0||rot==2)        //对应两种方向
        {
            SkewingSquare(obj,1, 1, 0);
            SkewingSquare(obj,2, -1, 1);
            SkewingSquare(obj,3, 0, 1);
        }
        else
        {
            SkewingSquare(obj,0, -1, 0);
            SkewingSquare(obj,1, -1, 1);
            SkewingSquare(obj,2, 0, 1);
            SkewingSquare(obj,3, 0, 2);
        }

    }

    void ChangeSquare()   //检测方块是否可以变形        
    {
        bool ison = false;
        switch(nowSquartType)       
        {
            case 0:                     //I形方块
                ison = CanChangeISquart();          
                break;
            case 1:                        //O形方块无法变形
                ison = false;      
                break;
            case 2:                         //T形方块
                ison = CanChangeTSquare();
                break;
            case 3:                        //L形方块
                ison = CanChangeLSquart();
                break;
            case 4:                         //J形方块
                ison = CanChangeJSquart();
                break;
            case 5:                          //S形方块
                ison = CanChangeSsquart();
                break;
            case 6:                           //Z形方块
                ison = CanChangeZsquart();
                break;
        }
        if (ison)       //检测是否可以变形
        {
            ResetSquare();      //重置所有小方块
            nowSquartRotType++;     //改变方块方向
            if (nowSquartRotType >= 4)
            {
                nowSquartRotType = 0;
            }
            SetPostion(nowSquart,nowSquartType,nowSquartRotType);       //重新设置四个小方块的位置
        }
    }

    bool CanChangeISquart()  //判断I形方块是否可以变形
    {
        bool result = false;
        switch (nowSquartRotType)
        {
            case 0:            
                if (CanChange(0,1)&&CanChange(0,2)&&CanChange(0,3))
                {
                    result = true;
                }
                break;
            case 1:
                if(CanChange(1,0)&&CanChange(2,0)&&CanChange(3,0))
                {
                    result = true;
                }
                break;
            case 2:
                if (CanChange(0, 1) && CanChange(0, 2) && CanChange(0, 3))
                {
                    result = true;
                }
                break;
            case 3:
                if (CanChange(1, 0) && CanChange(2, 0) && CanChange(3, 0))
                {
                    result = true;
                }
                break;
        }
        return result;
    }
    bool CanChangeTSquare()     //判断T形方块是否可以变形
    {
       bool result = false;
      switch(nowSquartRotType)
        {
            case 0:
                if(CanChange(0,2)&&CanChange(1,1))
                {
                    result = true;
                }              
                break;
            case 1:
                if(CanChange(-1,1))
                {
                    result = true;
                }
                break;
            case 2:
                if(CanChange(0,2))
                {
                    result = true;
                }
                break;
            case 3:
                if(CanChange(-1,0)&&CanChange(1,0))
                {
                    result = true;
                }
                break;
        }
        return result;
    }

    bool CanChangeLSquart()     //判断L形方块是否可以变形
    {
        bool result = false;
        switch (nowSquartRotType)
        {
            case 0:
                if (CanChange(0, 1) && CanChange(1, 1))
                {
                    result = true;
                }
                break;
            case 1:
                if (CanChange(-1, 2)&&CanChange(0,2))
                {
                    result = true;
                }
                break;
            case 2:
                if (CanChange(-1, 0)&&CanChange(1,0)&&CanChange(1,1))
                {
                    result = true;
                }
                break;
            case 3:
                if (CanChange(-1, 2) && CanChange(-1, 1))
                {
                    result = true;
                }
                break;
        }
        return result;
    }

    bool CanChangeJSquart()     //判断J形方块是否可以变形
    {
        bool result = false;
        switch (nowSquartRotType)
        {
            case 0:
                if (CanChange(-1, 1) && CanChange(1, 0))
                {
                    result = true;
                }
                break;
            case 1:
                if (CanChange(0, 1) && CanChange(0, 2)&&CanChange(1,2))
                {
                    result = true;
                }
                break;
            case 2:
                if (CanChange(-1, 1) && CanChange(1, 0) && CanChange(1, 1))
                {
                    result = true;
                }
                break;
            case 3:
                if (CanChange(-1, 0) && CanChange(0, 2))
                {
                    result = true;
                }
                break;
        }
        return result;
    }

    bool CanChangeSsquart()        //判断S形方块是否可以变形
    {
        bool result = false;
        if (nowSquartRotType==0||nowSquartRotType==2)
        {
            if(CanChange(-1,1)&&CanChange(-1,2))
            {
                result = true;
            }
        }
        else
        {
            if(CanChange(-1,0)&&CanChange(1,1))
            {
                result= true;
            }
        }
        return result;
    }

    bool CanChangeZsquart()        //判断S形方块是否可以变形
    {
        bool result = false;
        if (nowSquartRotType == 0 || nowSquartRotType == 2)
        {
            if (CanChange(-1, 0) && CanChange(0, 2))
            {
                result = true;
            }
        }
        else
        {
            if (CanChange(0, 0) && CanChange(1, 0))
            {
                result = true;
            }
        }
        return result;
    }

    bool CanChange(int index1,int index2)        //检查该区域是否有方块
    {

        int row = myRow[4] + index2;
        int col = myCol[4] + index1;
       
        
        if((row>=0)&&(row<20 )&& (col>=0) && (col<10))           //避免数组越界
        {
            if(myArray[row,col]!=1)                 //该区域没有方块
            {               
                return true;
            }
            else
            {
                return false;
            }          
        }
        else
        {         
            return false;
        }
        
    }

    void SquareMove()  //方块移动
    {
        
        if (Input.GetKeyDown(KeyCode.S))        //按下S键，加快下落速度
        {
            tmpdowntime = tmpdowntime / 4;

        }
        if (Input.GetKeyUp(KeyCode.S))          //速度恢复
        {

            tmpdowntime = tmpdowntime * 4;
        }
        if (Time.time - gameTime >= tmpdowntime)    //每隔固定时间，方块下落
        {
            gameTime = Time.time;
           
            DowmMove();         

        }
        if (Input.GetKey(KeyCode.A))                //方块左移
        {
            if (Time.time - tmplifttime >= 0.1f)        //最短触发时间0.1s
            {
                tmplifttime = Time.time;
                LiftMove();
            }
        }
        else if (Input.GetKey(KeyCode.D))        //方块右移
        {
            if (Time.time - tmplifttime >= 0.1f)        //最短触发时间0.1s
            {
                tmplifttime = Time.time;
                RightMove();
            }
        }
        if(Input.GetKeyDown(KeyCode.W))         //方块变形
        {
            if (nowSquart != null)
            {
                ChangeSquare();
            }
        }
       
    }
    void DowmMove()         //方块下移
    {
        for(int i=0;i<4;i++)                //判断所有的四个方块下降是否有障碍物
        {
            if(myRow[i]>0)                  //避免数组越界
            {
                if(myArray[myRow[i]-1,myCol[i]]==0)                     
                {
                    moveTypeD[i] = true;
                }
            }
        }
        if (moveTypeD[0] && moveTypeD[1] && moveTypeD[2] && moveTypeD[3])
        {
            for (int i = 0; i < 4; i++)             //四个小方块下移一格
            {
                moveTypeD[i] = false;
                nowSquart[i].transform.Translate(Vector3.down);
                myRow[i]--;                         
            }
            myRow[4]--;
        }
        else                            //方块已降落到底
        {
            Bottom();
        }
        IntoMoveTypeOp();
    }
    void LiftMove()     //方块左移
    {
        for (int i = 0; i < 4; i++)         //判断四个小方块左移是否有障碍
        {
            if (myCol[i] > 0)               //避免数组越界
            {
                if (myArray[myRow[i], myCol[i]-1] == 0)
                {
                    moveTypeL[i] = true;
                }
            }
        }
        if (moveTypeL[0] && moveTypeL[1] && moveTypeL[2] && moveTypeL[3])       //向左移动一格
        {
            for (int i = 0; i < 4; i++)
            {
                moveTypeL[i] = false;
                nowSquart[i].transform.Translate(Vector3.left);
                myCol[i]--;
            }
            myCol[4]--;
        }
        IntoMoveTypeOp();
    }

    void RightMove()    //方块右移
    {
        for (int i = 0; i < 4; i++)             
        {
            if (myCol[i] <9)            //避免数组越界
            {
                if (myArray[myRow[i], myCol[i] + 1] == 0)
                {
                    moveTypeR[i] = true;
                }
            }
        }
        if (moveTypeR[0] && moveTypeR[1] && moveTypeR[2] && moveTypeR[3])
        {
            for (int i = 0; i < 4; i++)
            {
                moveTypeR[i] = false;                
                nowSquart[i].transform.Translate(Vector3.right);    //将方块右移
                myCol[i]++;
            }
            myCol[4]++;
        }
        IntoMoveTypeOp();
    }

    void Bottom()      //方块移动到底部
    {
        
        for(int i=0;i<4;i++)
        {
            allSquart[myRow[i]][myCol[i]] = nowSquart[i];
            myArray[myRow[i], myCol[i]] = 1;
            nowSquart[i] = null;         
        }
        if(IsGameOver())       //游戏结束
        {
            
            Time.timeScale=0f;
            TipsWin.SetActive(true);
        }
        else           //加载下一个方块
        {
            AddList();
            NextSquart();
          
        }
    }
    bool IsGameOver()       //检测是否游戏结束
    {
        bool result = false;
        for (int i = 0; i < 10; i++)
        {
            if (myArray[19, i] == 1)
            {
                result = true;
            }
        }
        return result;
    }
    void AddList()        
    {
        for(int i = 0; i < 20; i++)     //统计要移除的行
        {
            int num = 0;
            for(int j=0;j<10;j++)       //该行都有方块时，进行移除
            {
                if (myArray[i, j] == 1)
                {
                    num++;          
                }
            }
            if (num == 10)
            {
                reList.Add(i);          //添加进容器
                score++;
            }    
        }
        if (reList.Count > 0)       //有需要移除的行时，进行移除操作
        {
            RemoveSquart();
            ShowScore();
        }
        

    }
    void RemoveSquart()             //移除方块
    {
        
        int count = reList.Count;           
        
        for (int k = 0; k < count; k++)         //循环移除所有行
        {
            {
                for (int i = 0; i < 10; i++)
                {
                    Destroy(allSquart[reList[0]][i]);       //删除该行的方块
                    myArray[reList[0], i] = 0;              //标记数组归为零
                }
                for (int i = reList[0]; i < 20; i++)        //讲该行上面的所有行下移
                {
                    for (int j = 0; j < 10; j++)                
                    {
                        allSquart[i][j] = allSquart[i + 1][j];      //将对象数组重新赋值
                        if (allSquart[i][j] != null)
                        {
                            allSquart[i][j].transform.Translate(Vector3.down);      //下移操作
                        }
                        myArray[i, j] = myArray[i + 1, j];          //标记数组重新赋值
                    }

                }
                reList.RemoveAt(0);         //移除容器里的数值
                for(int i = 0; i < reList.Count; i++)       //剩余需操作行数减一
                {
                    reList[i]--;
                }

            }
        }
      
    }

    public void ExitGame()      //退出游戏
    {
        Application.Quit();
    }

    public void LoadGame()  //重新开始游戏
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
       
    }

    void ShowScore()    //显示分数
    {
        scoretext.text ="分数："+ score.ToString();
    }



   
}

