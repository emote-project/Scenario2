// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by jni4net. See http://jni4net.sourceforge.net/ 
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

package emotecommonmessages;

@net.sf.jni4net.attributes.ClrInterface
public interface ILearnerModelToIMEvents {
    
    //<generated-interface>
    @net.sf.jni4net.attributes.ClrMethod("(LSystem/String;LSystem/String;)V")
    void learnerModelValueUpdateAfterAffectPerceptionUpdate(java.lang.String LearnerStateInfo_learnerState, java.lang.String AffectPerceptionInfo_AffectiveStates);
    //</generated-interface>
    
    @net.sf.jni4net.attributes.ClrMethod("(LSystem/String;)V")
    void LearnerStateInfo(java.lang.String LearnerStateInfo_learnerState);
}