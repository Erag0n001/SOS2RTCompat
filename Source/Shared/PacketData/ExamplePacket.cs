namespace Shared 
{
    // A class that can be sent over the network.

    public class ExampleData 
    {
        // Every field that we put in here will be sent over the network when we create an instance of the class.
        // In this case, we are only sending the "_toLog" variable.

        public string _toLog;
    }
}