package md5cc2b321e93bfa5be8efd8aa79e8cad01;


public class Dashboard
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("ZSProduct.Dashboard, ZSProduct, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Dashboard.class, __md_methods);
	}


	public Dashboard () throws java.lang.Throwable
	{
		super ();
		if (getClass () == Dashboard.class)
			mono.android.TypeManager.Activate ("ZSProduct.Dashboard, ZSProduct, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
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
