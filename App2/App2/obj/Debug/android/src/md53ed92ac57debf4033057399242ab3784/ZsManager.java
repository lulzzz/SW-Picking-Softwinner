package md53ed92ac57debf4033057399242ab3784;


public class ZsManager
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("App2.Modal.ZsManager, App2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ZsManager.class, __md_methods);
	}


	public ZsManager () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ZsManager.class)
			mono.android.TypeManager.Activate ("App2.Modal.ZsManager, App2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public ZsManager (android.content.Context p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == ZsManager.class)
			mono.android.TypeManager.Activate ("App2.Modal.ZsManager, App2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
