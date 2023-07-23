using Amazon;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambdaAuthentication;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(string input, ILambdaContext context)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) 
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Function>() //gets secret for this function. Checks after appsettings.json which is priority
            .AddEnvironmentVariables()
            .Build();

        var accessKey = config.GetValue<string>("AccessKey");
        var secret = config.GetValue<string>("Secret");
        var prefix = config.GetValue<string>("prefix");

        //AmazonSQSClient comes from amazon SDK 
        var credentials = new BasicAWSCredentials(accessKey, secret); //allows access key and basic key
        var client = new AmazonSQSClient(credentials, RegionEndpoint.USEast1);
        //var client = new AmazonSQSClient();

        var request = new SendMessageRequest()
        {
            QueueUrl = "https://sqs.us-east-1.amazonaws.com/816454053517/youtube-demo",
            MessageBody = input
        };

        var response = await client.SendMessageAsync(request);

        return $"{prefix} {input.ToUpper()} - {response.HttpStatusCode}";
    }
}
