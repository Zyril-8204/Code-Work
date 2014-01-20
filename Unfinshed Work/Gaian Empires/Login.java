package Game;

import java.awt.GridBagConstraints;
import java.awt.Insets;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.math.BigInteger;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.logging.Level;
import java.util.logging.Logger;

import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JTextField;

import Game.GameWindow;

public class Login extends JPanel {
	
	private static final long serialVersionUID = 1L;
	private static final String btnLoginText = "Login User";
	private static final String btnCreateUserText = "Create New User";
	private static final String btnCreateText = "Create User";
	private static final String btnCancelText = "Cancel";
	private static final String lblLoginText = "User Name: ";
	private static final String lblPasswordText = "Password: ";
	private static final String lblEmpireNameText = "Empire Name: ";
	private static final String lblRaceText = "Race: ";
	private static final String lblRulerText = "Ruler Name: ";
	private static final String lblPersonalityText = "Personality: ";

	// the combo boxes information
	private static final String[] RACES = { "Dwarf", "Elf", "Gnome", "Human"};
	private static final String[] PERSONALITIES = { "Cleric", "Harvester", "Merchant", "Warrior"};

	// jdbc connection string
	private static final String PROD_CONNECTION_URL = "jdbc:mysql://gaianempires.servegame.com:3306/gaian";
	private static final String DEV_CONNECTION_URL = "jdbc:mysql://localhost:3306/gaian";
	private static final String CONNECTION_PASS = "TXDcK6x2FLFtXPNbqEEM";
	private static final String CONNECTION_USER = "GaianEmpires";
	
	private static final String salt = "9414|\\|_3/\\/\\P1R35";

	private static JButton login, createUser, create, cancel;
	private static JLabel userNameLabel, passwordLabel, empireNameLabel, raceLabel, personalityLabel, rulerLabel;
	private static JTextField userNameField, passwordField, empireNameField, rulerNameField;
	private static JComboBox raceDrop, personalityDrop;

	public static void CreateLogin() {
		GameWindow.gBC.insets = new Insets(5,5,5,5);

		userNameLabel = new JLabel(lblLoginText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 0;
		GameWindow.frame.add(userNameLabel, GameWindow.gBC);
		userNameLabel.setVisible(false);

		userNameField = new JTextField(10);
		GameWindow.gBC.gridx = 1;
		GameWindow.gBC.gridy = 0;
		GameWindow.frame.add(userNameField, GameWindow.gBC);
		userNameField.requestFocus();
		userNameField.setVisible(false);

		passwordLabel = new JLabel(lblPasswordText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 1;
		GameWindow.frame.add(passwordLabel, GameWindow.gBC);
		passwordLabel.setVisible(false);

		passwordField = new JTextField(10);
		GameWindow.gBC.gridx = 1;
		GameWindow.gBC.gridy = 1;
		GameWindow.frame.add(passwordField, GameWindow.gBC);
		passwordField.setVisible(false);

		empireNameLabel = new JLabel(lblEmpireNameText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 2;
		GameWindow.frame.add(empireNameLabel, GameWindow.gBC);
		empireNameLabel.setVisible(false);

		empireNameField = new JTextField(10);
		GameWindow.gBC.gridx = 1;
		GameWindow.gBC.gridy = 2;
		GameWindow.frame.add(empireNameField, GameWindow.gBC);
		empireNameField.setVisible(false);
		
		rulerLabel = new JLabel(lblRulerText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 3;
		GameWindow.frame.add(rulerLabel, GameWindow.gBC);
		rulerLabel.setVisible(false);
		
		rulerNameField = new JTextField(10);
		GameWindow.gBC.gridx = 1;
		GameWindow.gBC.gridy = 3;
		GameWindow.frame.add(rulerNameField, GameWindow.gBC);
		rulerNameField.setVisible(false);

		raceLabel = new JLabel(lblRaceText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 4;
		GameWindow.frame.add(raceLabel, GameWindow.gBC);
		raceLabel.setVisible(false);

		raceDrop = new JComboBox();
		for (int i = 0; i < RACES.length; i++) {
			raceDrop.addItem(RACES[i]);
		}
		GameWindow.gBC.gridx = 1;
		GameWindow.gBC.gridy = 4;
		GameWindow.frame.add(raceDrop, GameWindow.gBC);
		raceDrop.setVisible(false);		

		personalityLabel = new JLabel(lblPersonalityText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 5;
		GameWindow.frame.add(personalityLabel, GameWindow.gBC);
		personalityLabel.setVisible(false);

		personalityDrop = new JComboBox();
		for (int i = 0; i < PERSONALITIES.length; i++) {
			personalityDrop.addItem(PERSONALITIES[i]);
		}
		GameWindow.gBC.gridx = 1;
		GameWindow.gBC.gridy = 5;
		GameWindow.frame.add(personalityDrop, GameWindow.gBC);
		personalityDrop.setVisible(false);

		login = new JButton(btnLoginText);
		GameWindow.gBC.gridx = 2;
		GameWindow.gBC.gridy = 3;
		GameWindow.gBC.gridwidth = 1;
		GameWindow.gBC.fill = GridBagConstraints.HORIZONTAL;
		GameWindow.frame.add(login, GameWindow.gBC);
		login.addActionListener(new UserLogin());
		login.setVisible(false);

		createUser = new JButton(btnCreateUserText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 3;
		GameWindow.gBC.gridwidth = 1;
		GameWindow.gBC.fill = GridBagConstraints.HORIZONTAL;
		GameWindow.frame.add(createUser, GameWindow.gBC);
		createUser.addActionListener(new UserCreationForm());
		createUser.setVisible(false);

		create = new JButton(btnCreateText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 6;
		GameWindow.gBC.gridwidth = 1;
		GameWindow.gBC.fill = GridBagConstraints.HORIZONTAL;
		GameWindow.frame.add(create, GameWindow.gBC);
		create.setVisible(false);
		create.addActionListener(new UserCreate());

		cancel = new JButton(btnCancelText);
		GameWindow.gBC.gridx = 2;
		GameWindow.gBC.gridy = 6;
		GameWindow.gBC.gridwidth = 1;
		GameWindow.gBC.fill = GridBagConstraints.HORIZONTAL;
		GameWindow.frame.add(cancel, GameWindow.gBC);
		cancel.setVisible(false);
		cancel.addActionListener(new UserCreationCancel());		
	}

	private static class UserCreationForm implements ActionListener {

		@Override
		public void actionPerformed(ActionEvent e) {
			empireNameLabel.setVisible(true);
			empireNameField.setVisible(true);
			rulerLabel.setVisible(true);
			rulerNameField.setVisible(true);
			raceLabel.setVisible(true);
			raceDrop.setVisible(true);
			personalityLabel.setVisible(true);
			personalityDrop.setVisible(true);
			login.setVisible(false);
			createUser.setVisible(false);
			create.setVisible(true);
			cancel.setVisible(true);
			userNameField.requestFocus();
			userNameField.setText(null);
			passwordField.setText(null);
		}
	}

	private static class UserCreate implements ActionListener {
		// catch for blank names and passwords
		// come up with a minimum user, password, empire name length (6 characters?)
		private static final String SQL_INSERT = "INSERT INTO user"
				+ " VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?,?)";
		private int myMaxId;

		@Override
		public void actionPerformed(ActionEvent e) {
			String user = userNameField.getText().toLowerCase();
			String strPass = passwordField.getText();
			String empireName = empireNameField.getText();
			String rulerName = rulerNameField.getText();
			String race = (String) raceDrop.getSelectedItem();
			String personality = (String) personalityDrop.getSelectedItem();
			long raceNum = raceDrop.getSelectedIndex();
			long personalityNum = personalityDrop.getSelectedIndex();
			long divisionID; 
			long empiresubID;
			long empireID;
			int usersReturned = 0;
			int empiresReturned = 0;
			String sqlSetPass = md5(strPass + salt);

			//open a database connection and return matching user name or empire name.
			try {
				Class.forName("com.mysql.jdbc.Driver");
			} catch(ClassNotFoundException ex) {
				Logger.getLogger(Login.class.getName()).log(Level.SEVERE, null,ex);
			} 
			try {
				Connection con = (Connection) DriverManager.getConnection(DEV_CONNECTION_URL,CONNECTION_USER, CONNECTION_PASS);
				Statement st = con.createStatement();
				String query = "SELECT * FROM `user` where username = '" + user + "'";
				//debugging
				ResultSet rs = st.executeQuery(query);
				while (rs.next()) {
					usersReturned ++;
				}
				String query2 = "SELECT * FROM `user` where empirename = '" + empireName + "'";
				//debugging
				ResultSet rs2 = st.executeQuery(query2);
				while (rs2.next()) {
					empiresReturned ++;
				}

			} catch(SQLException ex) {
				Logger.getLogger(Login.class.getName()).log(Level.SEVERE, null, ex);
			}

			// we will error if these are matched before we set a database value
			if (usersReturned != 0) {
				JOptionPane.showMessageDialog(null, "That user name is already taken please select another. ","User Name Error!",1);
			} else if(user.length() <= 5) {
				JOptionPane.showMessageDialog(null, "That user name is to short. \nPlease make sure it is 6 characters or more. ","User Name Error!",1);
			} else if(user.contains(" ")) {
				JOptionPane.showMessageDialog(null, "User names can not contain spaces! ","User Name Error!",1);
			} else if(strPass.length() <= 6) {
				JOptionPane.showMessageDialog(null, "That password is to short. \nPlease make sure it is 7 characters or more. ","Password Error!",1);
			} else if(strPass.contains(" ")) {
				JOptionPane.showMessageDialog(null, "Passwords can not contain spaces! ","Password Error!",1);
			} else if(empiresReturned !=0) {
				JOptionPane.showMessageDialog(null, "That empire name is already taken please select another. ","Empire Name Error!",1);
			} else if(empireName.length() <= 4) {
				JOptionPane.showMessageDialog(null, "That empire name is to short. \nPlease make sure it is 5 characters or more. ","Empire Name Error!",1);
			} else {
				try {
					Class.forName("com.mysql.jdbc.Driver");
				} catch(ClassNotFoundException ex) {
					Logger.getLogger(Login.class.getName()).log(Level.SEVERE, null,ex);
				} 
				try {
					Connection con = (Connection) DriverManager.getConnection(DEV_CONNECTION_URL,CONNECTION_USER, CONNECTION_PASS);
					Statement st = con.createStatement();
					String query = "SELECT MAX(ID) FROM `user`";
					ResultSet rs = st.executeQuery(query);
					rs.next();
					myMaxId = rs.getInt(1);
					myMaxId++;				
				} catch(SQLException ex) {
					Logger.getLogger(Login.class.getName()).log(Level.SEVERE, null, ex);
				}
				if (myMaxId == 0) {
					myMaxId = 1;
				}

				divisionID = myMaxId % 40;
				if (divisionID == 0) {
					divisionID = 1;
				}
				empiresubID = (myMaxId / 1600) + 1;
				empireID = myMaxId;

				try {
					Class.forName("com.mysql.jdbc.Driver");
				} catch(ClassNotFoundException ex) {
					Logger.getLogger(Login.class.getName()).log(Level.SEVERE, null,ex);
				} 
				try {
					Connection con = (Connection) DriverManager.getConnection(DEV_CONNECTION_URL,CONNECTION_USER, CONNECTION_PASS);
					PreparedStatement statement = con.prepareStatement(SQL_INSERT);
					statement.setLong(1, myMaxId);
					statement.setLong(2, divisionID);
					statement.setLong(3, empiresubID);
					statement.setLong(4, empireID);
					statement.setString(5, user);
					statement.setString(6, sqlSetPass);
					statement.setString(7, empireName);
					statement.setString(8, race);
					statement.setString(9, personality);
					statement.setLong(10, raceNum);
					statement.setLong(11, personalityNum);
					statement.setString(12, rulerName);
					statement.executeUpdate();
					if (statement != null) statement.close(); 
					if (con != null) con.close();			        
				} catch(SQLException ex) {
					Logger.getLogger(Login.class.getName()).log(Level.SEVERE, null, ex);
				}

				//set up the user class on new user create.
				User.setUserName(user);
				User.setDivision(divisionID);
				User.setEmpireId(empireID);
				User.setEmpireName(empireName);
				User.setId(myMaxId);
				User.setPersonality(personalityNum);
				User.setPersonalityName(personality);
				User.setRace(raceNum);
				User.setRaceName(race);
				User.setSubDivision(empiresubID);
				User.setRulerName(rulerName);
				JOptionPane.showMessageDialog(null, "User Created Successfully! ", "User Created", JOptionPane.PLAIN_MESSAGE);
				hideLogin();
			}
		}
	}

	private static class UserCreationCancel implements ActionListener {

		@Override
		public void actionPerformed(ActionEvent e) {
			empireNameLabel.setVisible(false);
			rulerLabel.setVisible(false);
			empireNameField.setVisible(false);
			rulerNameField.setVisible(false);
			raceLabel.setVisible(false);
			raceDrop.setVisible(false);
			personalityLabel.setVisible(false);
			personalityDrop.setVisible(false);
			login.setVisible(true);
			createUser.setVisible(true);
			create.setVisible(false);
			cancel.setVisible(false);
			userNameField.requestFocus();
			passwordField.setText(null);
			userNameField.setText(null);
			empireNameField.setText(null);
			rulerNameField.setText(null);
			raceDrop.setSelectedIndex(0);
			personalityDrop.setSelectedIndex(0);
		}
	}

	private static class UserLogin implements ActionListener {

		@Override
		public void actionPerformed(ActionEvent e) {
			String user = userNameField.getText().toLowerCase();
			String pass = passwordField.getText();
			String empireName = "null";
			String race = "null";
			String personality ="null";
			String rulerName ="null";
			long raceNum = -1;
			long personalityNum = -1;
			long divisionID = -1; 
			long empiresubID= -1;
			long empireID= -1;
			long ID = -1;

			String strpass = md5(pass+salt);

			try {
				Class.forName("com.mysql.jdbc.Driver");
			} catch(ClassNotFoundException ex) {
				Logger.getLogger(Login.class.getName()).log(Level.SEVERE, null,ex);
			}
			try {
				Connection con = (Connection) DriverManager.getConnection(DEV_CONNECTION_URL,CONNECTION_USER, CONNECTION_PASS);
				Statement st = con.createStatement();
				String query = "SELECT pass FROM User where username='"+user+"'";
				ResultSet rs = st.executeQuery(query);

				if (rs.next()) {
					String dbpass = rs.getString(1);
					if (dbpass.equals(strpass)) {
						JOptionPane.showMessageDialog(null, "Login Successful! ", "Success", JOptionPane.PLAIN_MESSAGE);
					} else {
						JOptionPane.showMessageDialog(null,  "Incorrect Password", "Error",1);
					}
				} else {
					JOptionPane.showMessageDialog(null, "Incorrect User Name","Error",1);
				}
			} catch(SQLException ex) {
				Logger.getLogger(Login.class.getName()).log(Level.SEVERE, null, ex);
			}

			try {
				Class.forName("com.mysql.jdbc.Driver");
			} catch(ClassNotFoundException ex) {
				Logger.getLogger(Login.class.getName()).log(Level.SEVERE, null,ex);
			}
			try {
				Connection con = (Connection) DriverManager.getConnection(DEV_CONNECTION_URL,CONNECTION_USER, CONNECTION_PASS);
				Statement st = con.createStatement();
				String query = "SELECT * FROM User where username='"+user+"'";
				ResultSet rs = st.executeQuery(query);

				if (rs.next()) {
					ID = rs.getLong(1);
					divisionID = rs.getLong(2);
					empiresubID = rs.getLong(3);
					empireID = rs.getLong(4);
					empireName = rs.getString(7);
					race = rs.getString(8);
					personality = rs.getString(9);
					raceNum =  rs.getLong(10);
					personalityNum =  rs.getLong(11);
					rulerName = rs.getString(12);
				}

			} catch(SQLException ex) {
				Logger.getLogger(Login.class.getName()).log(Level.SEVERE, null, ex);
			}
			if (ID != -1){
				//set up the user class on new user create.
				User.setUserName(user);
				User.setDivision(divisionID);
				User.setEmpireId(empireID);
				User.setEmpireName(empireName);
				User.setId(ID);
				User.setPersonality(personalityNum);
				User.setPersonalityName(personality);
				User.setRace(raceNum);
				User.setRaceName(race);
				User.setSubDivision(empiresubID);
				User.setRulerName(rulerName);
			}
			
				/* 	Debugging user
			 	System.out.println(User.getUserName());
				System.out.println(User.getDivision());
				System.out.println(User.getEmpireId());
				System.out.println(User.getEmpireName());
				System.out.println(User.getId());
				System.out.println(User.getPersonality());
				System.out.println(User.getPersonalityName());
				System.out.println(User.getRace());
				System.out.println(User.getRaceName());
				System.out.println(User.getSubDivision());
				*/
				hideLogin();
		}
	}

	public static void showLogin() {
		userNameLabel.setVisible(true);
		passwordLabel.setVisible(true);
		userNameField.setVisible(true);
		passwordField.setVisible(true);
		empireNameLabel.setVisible(false);
		empireNameField.setVisible(false);
		rulerLabel.setVisible(false);
		rulerNameField.setVisible(false);
		raceLabel.setVisible(false);
		raceDrop.setVisible(false);
		personalityLabel.setVisible(false);
		personalityDrop.setVisible(false);
		login.setVisible(true);
		createUser.setVisible(true);
		create.setVisible(false);
		cancel.setVisible(false);
		userNameField.requestFocus();
		passwordField.setText(null);
		userNameField.setText(null);
		empireNameField.setText(null);
		raceDrop.setSelectedIndex(0);
		personalityDrop.setSelectedIndex(0);
	}

	public static void hideLogin() {
		userNameLabel.setVisible(false);
		passwordLabel.setVisible(false);
		userNameField.setVisible(false);
		passwordField.setVisible(false);
		empireNameLabel.setVisible(false);
		empireNameField.setVisible(false);
		rulerLabel.setVisible(false);
		rulerNameField.setVisible(false);
		raceLabel.setVisible(false);
		raceDrop.setVisible(false);
		personalityLabel.setVisible(false);
		personalityDrop.setVisible(false);
		login.setVisible(false);
		createUser.setVisible(false);
		create.setVisible(false);
		cancel.setVisible(false);
		//Game.loginDone = true;
	}

	public static String md5(String input) {

		String md5 = null;

		if(null == input) return null;

		try {     
			//Create MessageDigest object for MD5
			MessageDigest digest = MessageDigest.getInstance("MD5");

			//Update input string in message digest
			digest.update(input.getBytes(), 0, input.length());

			//Converts message digest value in base 16 (hex)
			md5 = new BigInteger(1, digest.digest()).toString(16);

		} catch (NoSuchAlgorithmException e) {
			e.printStackTrace();
		}
		return md5;
	}
}