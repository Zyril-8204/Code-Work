package Game;

import java.awt.Graphics2D;
import java.awt.GridBagConstraints;
import java.awt.Insets;

import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JTextField;

public class Interface {
	private static String empireName;
	private static String rulerName;
	private static String currentAge;
	private static String raceName;
	private static String personalityName;
	private static final String STRENGTHstr = "Empire Strength: ";
	private static final String DEFENSEstr = "Empire Defense: ";
	private static final String EXPstr = "EXP to Level: ";
	private static final String AGEstr = "EXP to New Age: ";
	private static final String MAPBTNstr = "MAP";
	private static final String MESSAGEBTNstr = "MESSAGES";
	private static final String ATTACKPLAYERBTNstr = "Attack Player";
	private static final String ATTACKNPCBTNstr = "Attack NPC";
	private static final String BUILDBTNstr = "Build Buildings";
	private static final String TRAINBTNstr = "Train Units";
	private static final String UPGRADEBLDSBTNstr = "Upgrade Buildings";
	private static final String UPGRADEUNITSBTNstr = "Upgrade Units";
	private static final String STOREBTNstr = "Store";
	private static final String LUMBERstr = "Lumber: ";
	private static final String GOLDstr = "Gold: ";
	private static final String DIAMONDSstr = "Diamonds: ";
	private static final String FOODstr = "Food: ";
	private static final String CURPOPULATIONstr = "Current Population: ";
	private static final String POPULATIONGROWstr = "Next Population Growth: ";
	private static final String MAXPOPULATIONstr = "Maximum Population: ";
	
	// our labels to put on UI
	private JLabel empireNameLbl, rulerNameLbl, curAgeLbl, raceNameLbl, personalityNameLbl, strengthLbl, defenseLbl,
					expLbl, ageLbl, curPopLbl, goldLbl, lumberLbl, diamondLbl, foodLbl, popGrowLbl, maxPopLbl;
	
	private JButton mapBtn, messageBtn, attackPlayerBtn, attackNpcBtn, buildBtn, trainBtn, upgradeBuildingsBtn,
					upgradeUnitsBtn, storeBtn;
	

	public static void CreateInterface() {
		//GameWindow.gBC.insets = new Insets(5,5,5,5);
/*
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

		raceLabel = new JLabel(lblRaceText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 3;
		GameWindow.frame.add(raceLabel, GameWindow.gBC);
		raceLabel.setVisible(false);

		raceDrop = new JComboBox();
		for (int i = 0; i < RACES.length; i++) {
			raceDrop.addItem(RACES[i]);
		}
		GameWindow.gBC.gridx = 1;
		GameWindow.gBC.gridy = 3;
		GameWindow.frame.add(raceDrop, GameWindow.gBC);
		raceDrop.setVisible(false);		

		personalityLabel = new JLabel(lblPersonalityText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 4;
		GameWindow.frame.add(personalityLabel, GameWindow.gBC);
		personalityLabel.setVisible(false);

		personalityDrop = new JComboBox();
		for (int i = 0; i < PERSONALITIES.length; i++) {
			personalityDrop.addItem(PERSONALITIES[i]);
		}
		GameWindow.gBC.gridx = 1;
		GameWindow.gBC.gridy = 4;
		GameWindow.frame.add(personalityDrop, GameWindow.gBC);
		personalityDrop.setVisible(false);

		login = new JButton(btnLoginText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 3;
		GameWindow.gBC.gridwidth = 1;
		GameWindow.gBC.fill = GridBagConstraints.HORIZONTAL;
		GameWindow.frame.add(login, GameWindow.gBC);
		login.addActionListener(new UserLogin());
		login.setVisible(false);

		createUser = new JButton(btnCreateUserText);
		GameWindow.gBC.gridx = 2;
		GameWindow.gBC.gridy = 3;
		GameWindow.gBC.gridwidth = 1;
		GameWindow.gBC.fill = GridBagConstraints.HORIZONTAL;
		GameWindow.frame.add(createUser, GameWindow.gBC);
		createUser.addActionListener(new UserCreationForm());
		createUser.setVisible(false);

		create = new JButton(btnCreateText);
		GameWindow.gBC.gridx = 2;
		GameWindow.gBC.gridy = 5;
		GameWindow.gBC.gridwidth = 1;
		GameWindow.gBC.fill = GridBagConstraints.HORIZONTAL;
		GameWindow.frame.add(create, GameWindow.gBC);
		create.setVisible(false);
		create.addActionListener(new UserCreate());

		cancel = new JButton(btnCancelText);
		GameWindow.gBC.gridx = 0;
		GameWindow.gBC.gridy = 5;
		GameWindow.gBC.gridwidth = 1;
		GameWindow.gBC.fill = GridBagConstraints.HORIZONTAL;
		GameWindow.frame.add(cancel, GameWindow.gBC);
		cancel.setVisible(false);
		cancel.addActionListener(new UserCreationCancel());		
	*/}
}
