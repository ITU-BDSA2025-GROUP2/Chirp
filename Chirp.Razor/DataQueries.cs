public class DataQueries{
    using var connection = new SqliteConnection("Data source=/tmp/chirp.db");

    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = "Select * from message limit 32";

    using var reader = command.ExecuteReader();


    public void testQuery(){
        while(reader.Read()){
            var message = reader.GetString();

            Console.WriteLine(message);
        }
    }
    
}