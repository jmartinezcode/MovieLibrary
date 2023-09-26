using System.ComponentModel;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32.SafeHandles;
using NLog;

string path = Directory.GetCurrentDirectory() + "\\nlog.config";
// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();

string file = "movies.csv";
string choice;

do
{
    Console.WriteLine("\n1) Display Movies");
    Console.WriteLine("2) Add Movie");
    Console.WriteLine("Any other key to exit");
    choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            DisplayMovies(file);
            break;
        case "2":
            AddMovie(file);
            break;
        default:
            Console.WriteLine("Goodbye!\n");
            break;
    }
} while (choice == "1" || choice == "2");
    


void AddMovie(string file)
{
    // Get last id from file and add 1 for next movie
    int id = GetLastMovieId(file) + 1;
    // prompt for movie info
    Console.Write("Enter movie title: ");
    string movieTitle = Console.ReadLine();
    // Prompt for year and verify valid input
    int movieYear;
    while (true)
    {
        Console.Write("Enter the year the movie was made: ");
        if (int.TryParse(Console.ReadLine(), out movieYear) && movieYear >= 1900 && movieYear <= DateTime.Now.Year)
        {            
            break;            
        }
        else
        {
            logger.Error($"Invalid year entered. Please enter a valid year between 1900 and {DateTime.Now.Year}");
        }
    }
    // concatenate title with year
    string titleWithYear = $"{movieTitle} ({movieYear})";
    // create list to store genres
    List<string> genresList = new List<string>();
    // Prompt for genres until user is done
    while (true)
    {
        Console.Write("Enter movie genre (or type 'done' to finish): ");
        string genre = Console.ReadLine();
        if (!string.Equals(genre, "done", StringComparison.OrdinalIgnoreCase))
        {
            // add to list
            genresList.Add(genre);
        }
        else
        {
            // done
            break;
        }   
    }   

    // concatenate all genres with "|"
    string movieGenres = string.Join("|", genresList);
    

    //Check for duplicate
    if (!IsDuplicateMovie(file, titleWithYear))
    {
        // create movie object
        var movie = new Movie { movieId = id, title = titleWithYear, genres = movieGenres };
        // add movie to csv
        using (var sw = new StreamWriter(file, append: true))
        using (var csv = new CsvWriter(sw, CultureInfo.InvariantCulture))
        {
            csv.WriteRecord(movie);
            // add a newline to separate records
            // csv.NextRecord(); // not 
            sw.WriteLine();
        }

        Console.WriteLine("Movie added successfully.");
    }
    else
    {
        // create logger message
        logger.Error("Movie with same title already exists.");
    }
}
static void DisplayMovies(string file)
{
    if (File.Exists(file))
    {
        using (var sr = new StreamReader(file))
        using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<Movie>();
            foreach (var movie in records)
            {
                Console.WriteLine($"\nID: {movie.movieId}");
                Console.WriteLine($"Title: {movie.title}");
                Console.Write("Genre(s): ");
                string[] genres = movie.genres.Split("|");
                foreach (var genre in genres)
                    Console.Write($"{genre} ");
                Console.WriteLine();
            }
        }
    }
}
static int GetLastMovieId(string file)
{
    int id = 0;
    if (File.Exists(file))
    {
        using (var sr = new StreamReader(file))
        using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<Movie>();
            foreach (var movie in records)
            {
                id = movie.movieId;
            }
        }
    }
    return id;
}
static bool IsDuplicateMovie(string file, string title)
{
    if(File.Exists(file))
    {
        using (var sr = new StreamReader(file))
        using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<Movie>();
            foreach (var movie in records)
            {
                if (movie.title.Equals(title, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }
    }
    return false;
}

public class Movie 
{
    public int movieId { get; set;  }
    public string title { get; set; }
    public string genres { get; set;  }
}

