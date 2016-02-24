Thalamus is a component integration framework, developed in order to support the development of interactive agents that can seamlessly integrate the agent's logic with components for both various embodiments (virtual or robotic) and mixed environments (virtual and physical).
It was written by Tiago Ribeiro (tiago.ribeiro(at)gaips.inesc-id.pt), in the context of the EU Fp7 LiREC (www.lirec.eu) and EMOTE (www.emote-project.eu) projects.


The following papers have been published in conferences, related to this framework:
Ribeiro, T., Vala, M., & Paiva, A. (2012, January). Thalamus: Closing the mind-body loop in interactive embodied characters. In Intelligent virtual agents (pp. 189-195). Springer Berlin Heidelberg.
Ribeiro, T., Vala, M., & Paiva, A. (2013, January). Censys: A Model for Distributed Embodied Cognition. In Intelligent Virtual Agents (pp. 58-67). Springer Berlin Heidelberg.
Ribeiro, T., Di Tullio, E., Corrigan, L. J., Jones, A., Papadopoulos, F., Aylett, R., Castellano, G., Paiva, A. (2014, to appear). Developing Interactive Embodied Characters using the Thalamus Framework: A Collaborative Approach.
Ribeiro, T., Di Tullio, E., Pereira, A., Paiva, A. (2014, to appear). From Thalamus to Scenica - High-level behaviour planning and managing for mixed-reality characters.


The Documentation folder contains a guide on how to start building yout Thalamus Modules.
There are also some example Modules in the "Example Modules" folder.

The basic workflow with Thalamus is described in the following steps.
- Define the Event Interfaces in libraries that can be shared by different Modules. 
- Develop each necessary Module, which can subscribe and publish to Event Interfaces. 
	In order to subscribe to an Interface, the Module just has to implement it. 
	The framework will automatically take care of gathering everything and communicating with the Master. 
- Run the ThalamusMaster GUI (ThalamusStandalone).
- Run each Module as a separate application, in the same sub-network as the Master. Each Module will automatically connect and register all their Events.
- Depending on the funcionally of each Module, they will start publishing Events, which will be received by other Modules that have subscribed to the same Events.
- Modules can publish scheduled Actions (like a BML block), which will be kept in the Master's internal Scheduler, until the corresponding trigger Event is published.