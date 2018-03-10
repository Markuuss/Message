using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Firebase.Xamarin.Database;
using System.Collections.Generic;
using com.refractored.fab;
using Firebase.Database;
using System;
using Android.Views;
using Firebase.Auth;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.Widget;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;

namespace XamarinChatApp
{
    [Activity(Label = "XamarinChatApp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : AppCompatActivity, IValueEventListener
    {
        DrawerLayout drawerLayout;
        NavigationView navigationView;
      
        private FirebaseClient firebase;
        private List<MessageContent> lstMessage = new List<MessageContent>();
        private ListView lstChat;
        private EditText edtChat;
        private com.refractored.fab.FloatingActionButton fab;
        private TextView user_name;

        public int MyResultCode = 1;


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }



        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);


            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            // Create ActionBarDrawerToggle button and add it to the toolbar  
            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.drawer_open, Resource.String.drawer_close);
            drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            //user_name = navigationView.FindViewById<TextView>(Resource.Id.navheader_username);
            //user_name.Text = FirebaseAuth.Instance.CurrentUser.Email;
            setupDrawerContent(navigationView); //Calling Function 

            firebase = new FirebaseClient(GetString(Resource.String.firebase_database_url));
            FirebaseDatabase.Instance.GetReference("chats").AddValueEventListener(this);

            fab = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fab);
            edtChat = FindViewById<EditText>(Resource.Id.input);
            lstChat = FindViewById<ListView>(Resource.Id.list_of_messages);
            


            fab.Click += delegate
            {
                PostMessage();
            };



            if (FirebaseAuth.Instance.CurrentUser == null)
                StartActivityForResult(new Android.Content.Intent(this, typeof(SignIn)), MyResultCode);
            else
            {
                Toast.MakeText(this, "Welcome " + FirebaseAuth.Instance.CurrentUser.Email, ToastLength.Short).Show();
                DisplayChatMessage();
            }
           
        }

        private async void PostMessage()
        {
            var items = await firebase.Child("chats").PostAsync(new MessageContent(FirebaseAuth.Instance.CurrentUser.Email, edtChat.Text));
            edtChat.Text = "";
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            DisplayChatMessage();
        }

        private async void DisplayChatMessage()
        {
            lstMessage.Clear();
            var items = await firebase.Child("chats")
                .OnceAsync<MessageContent>();

            foreach (var item in items)
                lstMessage.Add(item.Object);
            ListViewAdapter adapter = new ListViewAdapter(this, lstMessage);
            lstChat.Adapter = adapter;
            lstChat.SmoothScrollToPositionFromTop(0, 50000);
        }
        void setupDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_messages:
                        {


                            break;
                        }
                    case Resource.Id.nav_home:
                        {

                            break;
                        }
                    case Resource.Id.nav_settings:
                        {
                           

                                break;
                        }

                    default: break;
                }
                drawerLayout.CloseDrawers();
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            navigationView.InflateMenu(Resource.Menu.bottombar_menu); //Navigation Drawer Layout Menu Creation
            return true;
        }
    }

}



