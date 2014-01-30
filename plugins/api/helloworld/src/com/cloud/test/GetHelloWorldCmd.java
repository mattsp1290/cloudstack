package com.cloud.test;
 
import javax.inject.Inject;
import org.apache.log4j.Logger;
 
import org.apache.cloudstack.api.BaseCmd;
import org.apache.cloudstack.api.APICommand;
import org.apache.cloudstack.api.Parameter;
 
@APICommand(name = "getHelloWorld", description="Get a message that says Hello World", responseObject = GetHelloWorldCmdResponse.class, includeInApiDoc=true)
public class GetHelloWorldCmd extends BaseCmd {
    public static final Logger s_logger = Logger.getLogger(GetHelloWorldCmd.class.getName());
    private static final String s_name = "gethelloworldresponse";
 
    @Parameter(name="example", type=CommandType.STRING, required=false, description="Just an example string that will be uppercased")
    private String example;
 
    public String getExample() {
        return this.example;
    }
 
    @Override
    public void execute()
    {
        GetHelloWorldCmdResponse response = new GetHelloWorldCmdResponse();
        if ( this.example != null ) {
            response.setExampleEcho(example);
        }
 
        response.setObjectName("helloworld"); // the inner part of the json structure
        response.setResponseName(getCommandName()); // the outer part of the json structure
 
        this.setResponseObject(response);
    }
 
    @Override
    public String getCommandName() {
        return s_name;
    }
 
    @Override
    public long getEntityOwnerId() {
        return 0;
    }
}