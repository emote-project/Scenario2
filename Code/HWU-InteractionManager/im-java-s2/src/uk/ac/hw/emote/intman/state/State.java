package uk.ac.hw.emote.intman.state;

public class State {
    // Singleton
    private static final State _inst = new State ();

    public static State getState () {
        return _inst;
    }
}
