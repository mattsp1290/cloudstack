package com.cloud.test;
 
import com.cloud.utils.component.PluggableService;
import java.util.List;
import java.util.ArrayList;
import org.apache.log4j.Logger;
import com.cloud.test.GetHelloWorldCmd;
import javax.annotation.PostConstruct;
import org.springframework.stereotype.Component;
import javax.ejb.Local;
 
@Component
@Local(value = { HelloWorldManager.class })
public class HelloWorldManagerImpl implements HelloWorldManager {
    private static final Logger s_logger = Logger.getLogger(HelloWorldManagerImpl.class);
 
    public HelloWorldManagerImpl() {
        super();
    }
 
    @Override
    public List<Class<?>> getCommands() {
        List<Class<?>> cmdList = new ArrayList<Class<?>>();
        cmdList.add(GetHelloWorldCmd.class);
        return cmdList;
    }
}