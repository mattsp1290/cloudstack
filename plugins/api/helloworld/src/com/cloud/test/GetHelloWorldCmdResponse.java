package com.cloud.test;
 
import org.apache.cloudstack.api.ApiConstants;
import com.cloud.serializer.Param;
import com.google.gson.annotations.SerializedName;
import org.apache.cloudstack.api.BaseResponse;
 
import java.util.Date;
import java.text.SimpleDateFormat;
 
@SuppressWarnings("unused")
public class GetHelloWorldCmdResponse extends BaseResponse {
    @SerializedName(ApiConstants.IS_ASYNC) @Param(description="true if api is asynchronous")
    private Boolean isAsync;
    @SerializedName("helloWorld") @Param(description="The message that says hello world")
    private String  helloWorld;
    @SerializedName("exampleEcho") @Param(description="An upper cased string")
    private String  exampleEcho;
 
    public GetHelloWorldCmdResponse(){
        this.isAsync   = false;
 
        SimpleDateFormat dateformatYYYYMMDD = new SimpleDateFormat("yyyyMMdd hh:mm:ss");
        this.setHelloWorld( "Hello World!");
    }
 
    public void setAsync(Boolean isAsync) {
        this.isAsync = isAsync;
    }
 
    public boolean getAsync() {
        return isAsync;
    }
 
    public void setHelloWorld(String msg) {
        this.helloWorld = msg;
    }
 
    public void setExampleEcho(String exampleEcho) {
        this.exampleEcho = exampleEcho.toUpperCase();
    }
}