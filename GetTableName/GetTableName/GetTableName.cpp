// GetTableName.cpp : 此檔案包含 'main' 函式。程式會於該處開始執行及結束執行。
//

#include <iostream>
#include<vector>
#include<unordered_set>
#include<fstream>
#include<sstream>
#include<string>
#include<algorithm>
#include<cctype>
using namespace std;
struct data_CRUD {
    string name;
    bool sql_insert = false;
    bool sql_select = false;
    bool sql_update = false;
    bool sql_delete = false;
};
//分割函數
const std::vector<std::string> split(const std::string& str, const char& delimiter) {
    std::vector<std::string> result;
    std::stringstream ss(str);
    std::string tok;

    while (std::getline(ss, tok, delimiter)) {
        result.push_back(tok);
    }
    return result;
}
//特殊字元判斷
bool is_unsupportChar(unsigned char c)
{   //32-43 45-47 59-64 91-94 96 123-126為特殊字元
    vector<int> ascii = { 32,44,45,47,59,64,91,94,123,126 };
    int runVector=0;
    while (runVector != ascii.size())
    {
        for (int i = ascii[runVector]; i <= ascii[runVector + 1]; i++)
        {
            if (c == i)
            {
                return true;
            }
        }
        runVector += 2;
    }
    if (c == 96)
    {
        return true;
    }
    else {
        return false;
    }
}
void deleteDBO(string *newStr)
{
    if (newStr->length() > 3 && newStr->substr(0, 3) == "dbo") //去除dbo.
    {
        newStr->erase(0, 3); //dbonv_back_mst   
    }
}
void getTable_insert(vector<string> data,int position, vector<string>* position_insert, unordered_set<string>*table_name)
{
    while (position != data.size() && data[position] != "into" )
    {
        position++;;
    }
    position++;
    if (position < data.size())
    {
        string newStr = "";
        for (int i = 0; i < data[position].length(); i++)
        {
            if (data[position][i] == '(')
            {
                break;
            }
            else if (is_unsupportChar(data[position][i])) {
                continue;
            }
            else {
                newStr += data[position][i];
            }
        }
        deleteDBO(&newStr);
        position_insert->push_back(newStr);
        table_name->insert(newStr);
    }
}
void getTable_select(vector<string> data, int position, vector<string>* position_select, unordered_set<string>* table_name)
{
    while (position < data.size() && data[position] != "from")
    {
        position++;
    }
    position++;
    if (position < data.size())
    {
        int key = 1,i=0;
        while (key!=0)
        {
            string newStr = "";
            while (i < data[position].length() && data[position][i] != ',') //做一直到遇到逗號
            {   //32-47 59-64  91-94 96 123-126為特殊字元
                if (is_unsupportChar(data[position][i]))
                {
                    i++;
                    continue;
                }
                newStr += data[position][i];
                i++;
            }
            deleteDBO(&newStr);
            if (data[position][i] == ',') //有逗號代表還有Table
            {
                key++; i++;
                if (i >= data[position].length()) {//table在下一個空格後
                    key--;
                    position++;
                    i = 0;
                }
            }
            else {
                position++; i = 0;
            }
            if (position<data.size() && data[position][i] == ',') key++; //逗號在下個空格後
            key--;
            if (newStr == "") continue;          //沒有抓到名稱
            position_select->push_back(newStr);
            table_name->insert(newStr);
        }
    }
}
void getTable_update(vector<string> data, int position, vector<string>* position_update, unordered_set<string>* table_name)
{
    int check_Set = position; //檢查Update後面第二個值是不是set (sql update語法)
    while (check_Set<data.size() && data[check_Set] != "set")
    {
        check_Set++;
    }
    if (check_Set < data.size())
    {
        string newStr = "";
        for (int i = 0; i < data[check_Set-1].length(); i++)
        {
            if (is_unsupportChar(data[check_Set - 1][i]))
            {
                continue;
            }
            newStr += data[check_Set - 1][i];
        }
        deleteDBO(&newStr);
        position_update->push_back(newStr);
        table_name->insert(newStr);
    }
}
void getTable_delete(vector<string> data, int position, vector<string>* position_delete, unordered_set<string>* table_name)
{
    if (position < data.size() && data[position + 1]=="from")
    {
        string newStr = "";
        for (int i = 0; i < data[position + 2].length(); i++)
        {
            if (is_unsupportChar(data[position + 2][i]))
            {
                continue;
            }
            newStr += data[position + 2][i];
        }
        deleteDBO(&newStr);
        position_delete->push_back(newStr);
        table_name->insert(newStr);
    }
}
void position_search(vector<string> *position_insert,
    vector<string> *position_select,
    vector<string> *position_update,
    vector<string> *position_delete,
    vector<string> data,
    unordered_set<string> *table_name
    )
{
    for (int i = 0; i < data.size(); i++)
    {
        if (data[i].find("insert")!=string::npos) 
        {
            getTable_insert(data, i, &*position_insert, &*table_name);
        }
        else if (data[i].find("select") != string::npos)
        {
            getTable_select(data, i, &*position_select, &*table_name);
        }
        else if (data[i].find("update") != string::npos) 
        {
            getTable_update(data, i, &*position_update, &*table_name);
        }
        else if (data[i].find("delete") != string::npos)
        {
            getTable_delete(data, i, &*position_delete, &*table_name);
        }
    }
}

vector<data_CRUD> FindTable(vector<string> lines)
{
    vector<data_CRUD> table;
    unordered_set<string> table_name;
    vector<string> position_insert;
    vector<string> position_select;
    vector<string> position_update;
    vector<string> position_delete;
    for (int i = 0; i < lines.size(); i++)
    {
        vector<string> data = split(lines[i], ' ');
        position_search(&position_insert, &position_select, &position_update, &position_delete, data, &table_name);
    }
    //將找到的Table，判斷CRUD並儲存
    for (auto& i : table_name) {
        data_CRUD content;
        if (i == "") continue;
        content.name = i;
        //尋找Insert裡面是否有出現
        if (find(position_insert.begin(), position_insert.end(), i) != position_insert.end())
        {
            content.sql_insert = true;
        }
        if (find(position_select.begin(), position_select.end(), i) != position_select.end())
        {
            content.sql_select = true;
        }
        if (find(position_update.begin(), position_update.end(), i) != position_update.end())
        {
            content.sql_update = true;
        }
        if (find(position_delete.begin(), position_delete.end(), i) != position_delete.end())
        {
            content.sql_delete = true;
        }
        table.push_back(content);
    }
    
    return table;
}
vector<string> removeAnnotations(vector<string> lines) //移除註解的部分
{
    vector<string> result;
    bool Annotation_lines = false;
    for (int i = 0; i < lines.size(); i++)
    {
        if (lines[i].find("//")!=string::npos)
        {
            string newStr = "";
            for (int i2 = 0; i < lines[i].length(); i2++)
            {
                if (lines[i][i2] == '/' && lines[i][i2+1] == '/')
                {
                    break;
                }
                newStr += lines[i][i2];
            }
            result.push_back(newStr);
        }
        else if (lines[i].find("/*") != string::npos)
        {
            Annotation_lines = true;
        }
        else if (lines[i].find("*/") != string::npos)
        {
            Annotation_lines = false;
        }
        else if(Annotation_lines==false) {
            result.push_back(lines[i]);
        }
    }
    return result;
}
void printAnswer(const data_CRUD i)
{
    cout << i.name << '\n';
    if (i.sql_insert==true)
    {
        cout << "Insert" << '\n';
    }
    if (i.sql_select == true)
    {
        cout << "Select" << '\n';
    }
    if (i.sql_update == true)
    {
        cout << "Update" << '\n';
    }
    if (i.sql_delete == true)
    {
        cout << "Delete" << '\n';
    }
    cout << '\n' << '\n';
}
//判斷中文
bool isChinese(string input)
{
    for (int i = 0; i < input.length(); i++)
    {
        unsigned char c= input[i];
        if (c >= 0x80)
        {
            return true;
        }
    }
    return false;
}
//處理輸入文件
vector<string> file_MainProgram(string path)
{
    string filename(path);
    vector<string> lines;
    string line = "",line_crud="";
    ifstream input_file(filename);
    if (!input_file.is_open()) {
        cerr << "Could not open the file - '"
            << filename << "'" << endl;
        exit(0);
    }
    bool find_crud = false;
    while (getline(input_file, line)) {
        transform(line.begin(), line.end(), line.begin(), tolower); //改小寫
        vector<string> newLine = split(line,' ');
        for (int i = 0; i < newLine.size(); i++) //只擷取有CRUD片段
        {
            if (newLine[i].find("insert") != string::npos ||
                newLine[i].find("select") != string::npos ||
                newLine[i].find("update") != string::npos ||
                newLine[i].find("delete") != string::npos
                )
            {
                lines.push_back(line_crud);
                line_crud = newLine[i];
            }
            else
            {
                if (newLine[i] == "" || newLine[i] == "\n" || newLine[i] == " " ) continue;
                if (isChinese(newLine[i]) == true) continue;
                line_crud += ' ' + newLine[i];
            }
        }
    }
    lines.push_back(line_crud);
    lines = removeAnnotations(lines); //去除註解
    return lines;
}
int main()
{
    //ExchangeGood_QueryLogic.cs AjaxCustomLogic.cs input.txt
    string path;
    //string path = "AjaxCustomLogic.cs";
    while (getline(cin, path))
    {
        cout << "--------------------------- - " << '\n';
        vector<string> lines = file_MainProgram(path);
        vector<data_CRUD> answer = FindTable(lines);
        for (const auto& i : answer)
        {
            printAnswer(i);
        }
        cout << "--------------------------- - " << '\n';
    }
    return 0;
}

// 執行程式: Ctrl + F5 或 [偵錯] > [啟動但不偵錯] 功能表
// 偵錯程式: F5 或 [偵錯] > [啟動偵錯] 功能表

// 開始使用的提示: 
//   1. 使用 [方案總管] 視窗，新增/管理檔案
//   2. 使用 [Team Explorer] 視窗，連線到原始檔控制
//   3. 使用 [輸出] 視窗，參閱組建輸出與其他訊息
//   4. 使用 [錯誤清單] 視窗，檢視錯誤
//   5. 前往 [專案] > [新增項目]，建立新的程式碼檔案，或是前往 [專案] > [新增現有項目]，將現有程式碼檔案新增至專案
//   6. 之後要再次開啟此專案時，請前往 [檔案] > [開啟] > [專案]，然後選取 .sln 檔案
