package md502f9f3d6b14c1c28d55665b227da2281;


public class ZsManager
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("ZSProduct.Modal.ZsManager, ZSProduct, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ZsManager.class, __md_methods);
	}


	public ZsManager () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ZsManager.class)
			mono.android.TypeManager.Activate ("ZSProduct.Modal.ZsManager, ZSProduct, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public ZsManager (android.content.Context p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == ZsManager.class)
			mono.android.TypeManager.Activate ("ZSProduct.Modal.ZsManager, ZSProduct, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
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
