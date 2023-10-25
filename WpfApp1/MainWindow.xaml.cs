using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Ink;
using System.Security.Cryptography;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Hold connection to DB
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();
            //Necessary to import some connection that we are working with
            string connectionString = ConfigurationManager.ConnectionStrings
                ["WpfApp1.Properties.Settings.SUNNYLESSONConnectionString"
                ].ConnectionString;

            //Initialize Connection
            sqlConnection = new SqlConnection(connectionString);
            displayStores();
            DisplayAllProduct();
           //DisplayStoryInventory();

        }

        private void displayStores() // function to update the store name
        {
            //Anytime you are accessing the database try surrounding the try and catch incase there is error

            try
            {
                //Next we have to define a query that pull the store names from database
                string query = "SELECT * FROM Store";

                //Several method to pull information from database
                //First
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
                    (query, sqlConnection); //this will connect to database run query and close database command connection //so we passing the query and sqlconnection
                                            //Next we are going to use our sqlDataAdapter and query connection to populate the list with different store names.
                using (sqlDataAdapter)
                {
                    DataTable storeTable = new DataTable();

                    //this is going to act interface between the database and the code 
                    sqlDataAdapter.Fill(storeTable); // putting information about our store

                    //Next we need to define the column we are going to display inour list
                    storeList.DisplayMemberPath = "Name";    //DisplayMemberPath - specifies the path to the display string property for each item. In your case, you'd set it to "Name"

                    //Next we define what will be unique about our list
                    //SelectedValuePath points to Id, which means you can get the Id of currently selected Employee by using SelectedValue.

                    storeList.SelectedValuePath = "Id";

                    //Contents we will use to populate the list 
                    storeList.ItemsSource = storeTable.DefaultView;
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //Function that will diplay our store inventory
        private void DisplayStoreInventory()
        {
            try
            {
                //Now we need to find a shoe that match a specific store so we are going to merge INNER JOIN

                //want to select everything from product p-alias to avoid
                //typing every item and then i want to say how i want this
                //product to be jon toegther and i want to look in
                //storeinventory the store inventory table SI for convenice and store inventory ID and Product, 
                //Also WHERE SI store Inventory ID is equal to the past store ID is provided and this is possible whenever they select te store

                string query = "SELECT * FROM Product p inner join StoreInventory si ON p.Id = si.ProductID WHERE si.StoreId = @StoreId";

                //To be able to use past variable, we have to use something called SQL COMMAND

                /*
                SQLCOMMAND in C# allow the user to query and send
                the commands to the database. SQL command is
                specified by the SQL connection object. 
                Two methods are used, ExecuteReader method 
                for results of query and ExecuteNonQuery 
                for insert, Update, and delete commands. 
                It is the method that is best for the different
                commands
                 */
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                //next we need to connect to database run the query and then close it
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                //Next we need to go populate the list box with store names
                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("StoreId", storeList.SelectedValue); //we using addwithvaue because we need to get the storeId that is selected so the query can actuall be executed using storelist.selectedValue
                    DataTable InventoryTable = new DataTable();
                    sqlDataAdapter.Fill(InventoryTable);
                    storeInventory.DisplayMemberPath = "Brand";
                    storeInventory.SelectedValuePath = "Id";
                    storeInventory.ItemsSource = InventoryTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void storeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayStoreInventory();
        }

        private void DisplayAllProduct() // function that will dispay all our store
        {
            try
            {
                //Now we need to find a shoe that match a specific store so we are going to merge INNER JOIN

                //want to select everything from product p-alias to avoid
                //typing every item and then i want to say how i want this
                //product to be jon toegther and i want to look in
                //storeinventory the store inventory table SI for convenice and store inventory ID and Product, 
                //Also WHERE SI store Inventory ID is equal to the past store ID is provided and this is possible whenever they select te store

                string query = "SELECT * FROM Product";

                //To be able to use past variable, we have to use something called SQL COMMAND

                /*
                SQLCOMMAND in C# allow the user to query and send
                the commands to the database. SQL command is
                specified by the SQL connection object. 
                Two methods are used, ExecuteReader method 
                for results of query and ExecuteNonQuery 
                for insert, Update, and delete commands. 
                It is the method that is best for the different
                commands
                 */
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                //next we need to connect to database run the query and then close it
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                //Next we need to go populate the list box with store names
                using (sqlDataAdapter)
                {
                    
                    DataTable ProductTable = new DataTable();
                    sqlDataAdapter.Fill(ProductTable);
                    productList.DisplayMemberPath = "Brand";
                    productList.SelectedValuePath = "Id";
                    productList.ItemsSource = ProductTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void addStoreClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //MessageBox.Show("addStoreClick"); //you can use this to test if at all your button work or not

                // Now we have to add store by add store name, street name state etc for the store name to show up
                //that why have to create SQL parameter list and store everything

                List<SqlParameter> parameters = new List<SqlParameter>()
            {
                //@Name - call reference, what type of data it is(SqlDbType.NVarChar),next you add Value and where you r getting it from(Value = storeName.Text)
               new SqlParameter("@Name", SqlDbType.NVarChar){Value = storeName.Text},
               new SqlParameter("@Street", SqlDbType.NVarChar){Value = storeStreet.Text},
               new SqlParameter("@City", SqlDbType.NVarChar){Value = storeCity.Text},
               new SqlParameter("@State", SqlDbType.NChar){Value = storeState.Text},
               new SqlParameter("@Zip", SqlDbType.Int){Value = storeZip.Text},
            };
                //Now we need to create a query access this sqlparameter we created
                string query = "INSERT INTO Store VALUES (@Name, @Street, @City, @State, @Zip)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddRange(parameters.ToArray());
                DataTable storeTable = new DataTable();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(storeTable);
            }
            catch (Exception ex) //always trying to catch an error
            {
                MessageBox.Show(ex.ToString());
            }
            finally //always close the database
            {
                sqlConnection.Close();
                displayStores();
            }
        }

        private void addInventoryClick(object sender, RoutedEventArgs e)
        {
            try
            {

                //MessageBox.Show("addStoreClick");
                string query = "INSERT INTO StoreInventory VALUES (@StoreId, @ProductId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@StoreId", storeList.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ProductId", productList.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
             catch (Exception ex) //always trying to catch an error
            {
                MessageBox.Show(ex.ToString());
            }
            finally //always close the database
            {
                sqlConnection.Close();
                DisplayStoreInventory();
            }
        }

        private void addProductClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //MessageBox.Show("addStoreClick"); //you can use this to test if at all your button work or not

                // Now we have to add store by add store name, street name state etc for the store name to show up
                //that why have to create SQL parameter list and store everything

                List<SqlParameter> parameters = new List<SqlParameter>()
            {
                //@Name - call reference, what type of data it is(SqlDbType.NVarChar),next you add Value and where you r getting it from(Value = storeName.Text)
               new SqlParameter("@Manufacturer", SqlDbType.NVarChar){Value = prodManu.Text},
               new SqlParameter("@Brand", SqlDbType.NVarChar){Value = prodBrand.Text},
              
            };
                //Now we need to create a query access this sqlparameter we created
                string query = "INSERT INTO Product VALUES (@Manufacturer, @Brand)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddRange(parameters.ToArray());
                DataTable productTable = new DataTable();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(productTable);
            }
            catch (Exception ex) //always trying to catch an error
            {
                MessageBox.Show(ex.ToString());
            }
            finally //always close the database
            {
                sqlConnection.Close();
                DisplayAllProduct(); //this will be call to update the product on inventory
            }
        }

        private void deleteStoreClick(object sender, RoutedEventArgs e)
        {
            try
            {

                string query = "DELETE FROM Store Id = @StoreId"; //meaning delete from our store Id at storeId
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@StoreId", storeList.SelectedValue); //going to delete whaever it deleted from list box
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex) //always trying to catch an error
            {
                MessageBox.Show(ex.ToString());
            }
            finally //always close the database
            {
                sqlConnection.Close();
                displayStores(); //this will be call to update the product on inventory
            }
        }

        private void deleteInventoryClick(object sender, RoutedEventArgs e)
        {
            try
            {

                string query = "DELETE FROM StoreInventory WHERE ProductId = @ProductId"; //meaning delete from our store Id at storeId
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ProductId", storeInventory.SelectedValue); //going to delete whaever it deleted from list box
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex) //always trying to catch an error
            {
                MessageBox.Show(ex.ToString());
            }
            finally //always close the database
            {
                sqlConnection.Close();
                DisplayStoreInventory(); //this will be call to update the product on inventory
            }
        }

        private void deleteProductClick(object sender, RoutedEventArgs e)
        {
            try
            {

                string query = "DELETE FROM Product Id = @ProductId"; //meaning delete from our store Id at storeId
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ProductId", productList.SelectedValue); //going to delete whaever it deleted from list box
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex) //always trying to catch an error
            {
                MessageBox.Show(ex.ToString());
            }
            finally //always close the database
            {
                sqlConnection.Close();
                DisplayAllProduct(); //this will be call to update the product on inventory
            }
        }
    }
}
