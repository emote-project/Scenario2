// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by jni4net. See http://jni4net.sourceforge.net/ 
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

package emoteenercitiesmessages;

import redstone.xmlrpc.XmlRpcArray;

@net.sf.jni4net.attributes.ClrTypeInfo
public final class IEnercitiesAIActions_ {
    
    //<generated-static>
    private static system.Type staticType;
    
    public static system.Type typeof() {
        return emoteenercitiesmessages.IEnercitiesAIActions_.staticType;
    }
    
    private static void InitJNI(net.sf.jni4net.inj.INJEnv env, system.Type staticType) {
        emoteenercitiesmessages.IEnercitiesAIActions_.staticType = staticType;
    }
    //</generated-static>
}

//<generated-proxy>
@net.sf.jni4net.attributes.ClrProxy
class __IEnercitiesAIActions extends system.Object implements emoteenercitiesmessages.IEnercitiesAIActions {
    
    protected __IEnercitiesAIActions(net.sf.jni4net.inj.INJEnv __env, long __handle) {
            super(__env, __handle);
    }
    
    @net.sf.jni4net.attributes.ClrMethod("(LSystem/String;)V")
    public native void StrategiesUpdated(java.lang.String StrategiesSet_strategies);
    
    @net.sf.jni4net.attributes.ClrMethod("([LSystem/String;)V")
    public native void BestActionPlanned(XmlRpcArray EnercitiesActionInfo_actionInfos);
    
    @net.sf.jni4net.attributes.ClrMethod("([LSystem/String;)V")
    public native void BestActionsPlanned(java.lang.String[] EnercitiesActionInfo_actionInfos);
    
    @net.sf.jni4net.attributes.ClrMethod("([D)V")
    public native void PredictedValuesUpdated(double[] values);

	@Override
	public void BestActionsPlanned(XmlRpcArray EnercitiesActionInfo_actionInfos) {
		// TODO Auto-generated method stub
		
	}
}
//</generated-proxy>
