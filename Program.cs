string file = "movies.csv";
string choice;

do
{
    Console.WriteLine("1) Display Movies");
    Console.WriteLine("2) Add Movie");
    choice = Console.ReadLine();
    if (choice == "1")
    {
        if (File.Exists(file))
        {
            using StreamReader sr = new StreamReader(file);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] arr = line.Split(",");
                string[] genres = arr[2].Split("|");
                Console.Write($"\nTitle: {arr[1]} \nGenre(s): ");
                foreach(var genre in genres) 
                {
                    Console.Write($"{genre} ");
                }
                Console.WriteLine();
            }
            

        }

    }

    else if (choice == "2") 
    {
        // generate ID number
        using StreamReader sr = new StreamReader(file);
        int id = 0;
        while (!sr.EndOfStream)
        {
            sr.ReadLine();
            id++;
        }
        //prompt for Movie Information

        
        StreamWriter sw = new StreamWriter(file, append: true);
        sw.WriteLine();
        sw.Close();

    }
} while (choice == "1" || choice == "2");

