// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by jni4net. See http://jni4net.sourceforge.net/ 
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

package emotecommonmessages;

@net.sf.jni4net.attributes.ClrTypeInfo
public final class IMapActions_ {
    
    //<generated-static>
    private static system.Type staticType;
    
    public static system.Type typeof() {
        return emotecommonmessages.IMapActions_.staticType;
    }
    
    private static void InitJNI(net.sf.jni4net.inj.INJEnv env, system.Type staticType) {
        emotecommonmessages.IMapActions_.staticType = staticType;
    }
    //</generated-static>
}

//<generated-proxy>
@net.sf.jni4net.attributes.ClrProxy
class __IMapActions extends system.Object implements emotecommonmessages.IMapActions {
    
    protected __IMapActions(net.sf.jni4net.inj.INJEnv __env, long __handle) {
            super(__env, __handle);
    }
    
    @net.sf.jni4net.attributes.ClrMethod("()V")
    public native void Click();
    
    @net.sf.jni4net.attributes.ClrMethod("()V")
    public native void Zoom();
    
    @net.sf.jni4net.attributes.ClrMethod("()V")
    public native void Pan();
    
    @net.sf.jni4net.attributes.ClrMethod("(DD)V")
    public native void Highlight(double x, double y);
    
    @net.sf.jni4net.attributes.ClrMethod("()V")
    public native void HighlightRightAnswer();
}
//</generated-proxy>